using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using MyMicroservice.Application.Common.Errors;
using MyMicroservice.Application.Common.Exceptions;
using MyMicroservice.Contracts.Requests;
using MyMicroservice.Contracts.Responses;
using MyMicroservice.Domain.ValueObjects;
using MyMicroservice.Enums;
using MyMicroservice.Infrastructure.Repositories;

namespace MyMicroservice.Application.Services;

public interface IUserService
{
    Task<UserJsonResponse> RegisterAsync(RegisterJsonRequest data);
    Task<AuthResponse> LoginAsync(LoginJsonRequest data);
}

public class UserService(
    IUserRepository userRepository,
    IRedisCacheService redisCacheService,
    IJwtService jwtService,
    ILogger<UserService> logger
) : IUserService
{
    /// <summary>
    ///     RegisterAsync - Регистрация нового пользователя
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    /// <exception cref="ApiException"></exception>
    public async Task<UserJsonResponse> RegisterAsync(RegisterJsonRequest data)
    {
        await ValidateUserUniquenessAsync(data.Email, data.Login);

        return await userRepository.CreateUser(new PreparedRegisterJsonRequest(
            data.Login,
            data.Email,
            data.Name,
            HashPassword(data.Password),
            Guid.NewGuid().ToString(),
            UserRolesExtensions.TryFromAlias(data.Role) ?? throw new ApiException(ApiError.InvalidRole)
        )) ?? throw new ApiException(ApiError.ValidationError);
    }

    /// <summary>
    ///     LoginAsync - Авторизация пользователя
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    /// <exception cref="ApiException"></exception>
    public async Task<AuthResponse> LoginAsync(LoginJsonRequest data)
    {
        var user = await userRepository.GetUserByLoginAsync(data.Login) ??
                   throw new ApiException(ApiError.UserNotFound);

        if (!VerifyPassword(data.Password, user.PasswordHash))
        {
            logger.LogWarning("Login failed: Invalid password for user {Login}", data.Login);
            throw new ApiException(ApiError.InvalidCredentials);
        }

        var roleName = UserRolesExtensions.FromRoleId(user.RoleId ?? 0) ?? "Unknown";

        var userData = new UserJsonResponse(
            user.Login,
            user.Email,
            user.Name,
            roleName,
            user.CreatedAt,
            user.UpdatedAt ?? DateTime.UtcNow
        );

        var token = jwtService.GenerateToken(user.ExtId, user.Login, user.Email, roleName);

        var sessionKey = GetSessionKey(user.ExtId);
        await redisCacheService.SetAsync(sessionKey, userData, TimeSpan.FromMinutes(30));

#if DEBUG
        var redisData = await redisCacheService.GetAsync<UserJsonResponse>(sessionKey);
        logger.LogInformation("Redis session created for user {Login}: {UserData}", data.Login, redisData);
#endif

        logger.LogInformation("User logged in successfully: {Login}", data.Login);

        return new AuthResponse(token, userData);
    }

    private static string GetSessionKey(string extId)
    {
        return $"u:{extId}";
    }

    /// <summary>
    ///     VerifyPassword - Проверяет хэши паролей
    /// </summary>
    /// <param name="password"></param>
    /// <param name="passwordHash"></param>
    /// <returns></returns>
    private static bool VerifyPassword(string password, string passwordHash)
    {
        var parts = passwordHash.Split('.');
        if (parts.Length != 2)
            return false;

        // Извлекаем сохраненную соль из хеша
        var salt = Convert.FromBase64String(parts[0]);
        var storedHash = parts[1];

        // Хешируем введенный пароль с ТОЙ ЖЕ СОЛЬЮ
        var computedHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password,
            salt,
            KeyDerivationPrf.HMACSHA256,
            100000,
            256 / 8));

        // Сравниваем только части с хешем (без соли)
        return computedHash == storedHash;
    }

    /// <summary>
    ///     HashPassword - Формирует хеш пароля
    /// </summary>
    /// <param name="password"></param>
    /// <returns></returns>
    private static string HashPassword(string password)
    {
        // Генерируем НОВУЮ соль только при создании пользователя
        var salt = new byte[128 / 8];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        var hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password,
            salt,
            KeyDerivationPrf.HMACSHA256,
            100000,
            256 / 8));

        return $"{Convert.ToBase64String(salt)}.{hashed}";
    }

    /// <summary>
    ///     Проверяет, что email и login уникальны. Если уже существует пользователь с таким email или login, выбрасывает
    ///     ApiException с ошибкой UserExist.
    /// </summary>
    /// <param name="email"></param>
    /// <param name="login"></param>
    /// <returns></returns>
    /// <exception cref="ApiException"></exception>
    /// TODO:
    /// - Оптимизировать в будущем через FluentValidation и проверку на уровне БД (уникальные индексы), 
    /// чтобы избежать гонок при одновременной регистрации нескольких пользователей с одинаковыми данными.
    private async Task ValidateUserUniquenessAsync(string email, string login)
    {
        // Проверяем email
        var existingUserByEmail = await userRepository.GetUserByEmailAsync(email);
        if (existingUserByEmail != null)
        {
            logger.LogWarning("Registration failed: Email {Email} already exists", email);
            throw new ApiException(ApiError.UserExist);
        }

        // Проверяем login
        var existingUserByLogin = await userRepository.GetUserByLoginAsync(login);
        if (existingUserByLogin != null)
        {
            logger.LogWarning("Registration failed: Login {Login} already exists", login);
            throw new ApiException(ApiError.UserExist);
        }
    }
}