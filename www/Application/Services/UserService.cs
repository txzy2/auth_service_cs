namespace MyMicroservice.Application.Services;

using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using MyMicroservice.Application.Common.Errors;
using MyMicroservice.Application.Common.Exceptions;
using MyMicroservice.Contracts;
using MyMicroservice.Contracts.Responses;
using MyMicroservice.Enums;
using MyMicroservice.Contracts.Requests;
using MyMicroservice.Infrastructure.Repositories;

public interface IUserService
{
    Task<UserJsonResponse> RegisterAsync(RegisterJsonRequest data);
    Task<UserJsonResponse> LoginAsync(LoginJsonRequest data);
}

public class UserService(
    IUserRepository userRepository,
    IRolesRepository rolesRepository,
    ILogger<UserService> logger
) : IUserService
{
    private readonly IUserRepository _repository = userRepository;
    private readonly IRolesRepository _rolesRepository = rolesRepository;
    private readonly ILogger<UserService> _logger = logger;

    public async Task<UserJsonResponse> RegisterAsync(RegisterJsonRequest data)
    {
        // Проверка на существующего пользователя
        var existingUser = await _repository.GetUserByEmailAsync(data.Email);
        if (existingUser != null)
        {
            throw new ApiException(ApiError.UserExist);
        }

        return await _repository.CreateUser(new PreparedRegisterJsonRequest(
            data.Login,
            data.Email,
            data.Name,
            HashPassword(data.Password),
            Guid.NewGuid().ToString(),
            UserRolesExtensions.TryFromAlias(data.Role) ?? throw new ApiException(ApiError.InvalidRole)
        )) ?? throw new ApiException(ApiError.ValidationError);
    }

    public async Task<UserJsonResponse> LoginAsync(LoginJsonRequest data)
    {
        var user = await _repository.GetUserByLoginAsync(data.Login) ?? throw new ApiException(ApiError.UserNotFound);

        // TODO: Добавить проверку пароля
        if (!VerifyPassword(data.Password, user.PasswordHash))
        {
            throw new ApiException(ApiError.InvalidCredentials);
        }

        return new UserJsonResponse(
            Login: user.Login,
            Email: user.Email,
            Name: user.Name,
            Role: UserRolesExtensions.FromRoleId(user.RoleId ?? 0) ?? "Unknown",
            CreatedAt: user.CreatedAt,
            UpdatedAt: user.UpdatedAt ?? DateTime.UtcNow
        );
    }

    private bool VerifyPassword(string password, string passwordHash)
    {
        var parts = passwordHash.Split('.');
        if (parts.Length != 2)
            return false;

        // Извлекаем сохраненную соль из хеша
        byte[] salt = Convert.FromBase64String(parts[0]);
        string storedHash = parts[1];

        // Хешируем введенный пароль с ТОЙ ЖЕ СОЛЬЮ
        string computedHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: salt, // ВАЖНО: используем сохраненную соль!
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 256 / 8));

        // Сравниваем только части с хешем (без соли)
        return computedHash == storedHash;
    }

    private static string HashPassword(string password)
    {
        // Генерируем НОВУЮ соль только при создании пользователя
        byte[] salt = new byte[128 / 8];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 256 / 8));

        return $"{Convert.ToBase64String(salt)}.{hashed}";
    }
}
