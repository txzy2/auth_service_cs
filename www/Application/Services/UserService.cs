using MyMicroservice.Contracts.Requests;
using MyMicroservice.Domain.Entities;
using MyMicroservice.Infrastructure.Repositories;

namespace MyMicroservice.Application.Services;

using MyMicroservice.Application.Common.Errors;
using MyMicroservice.Application.Common.Exceptions;

public interface IUserService
{
    Task<string> RegisterAsync(RegisterJsonRequest data);
    Task<User> LoginAsync(LoginJsonRequest data);
}

public class UserService(IUserRepository repository) : IUserService
{
    private readonly IUserRepository _repository = repository;

    public async Task<string> RegisterAsync(RegisterJsonRequest data)
    {
        // Проверка на существующего пользователя
        var existingUser = await _repository.GetUserByEmailAsync(data.Email);
        if (existingUser != null)
        {
            throw new ApiException(ApiError.UserExist);
        }

        var userId = await _repository.CreateUser(data);
        return $"User {data.Login} registered successfully with ID: {userId}";
    }

    public async Task<User> LoginAsync(LoginJsonRequest data)
    {
        var user = await _repository.GetUserByEmailAsync(data.Email) ?? throw new ApiException(ApiError.UserNotFound);

        // TODO: Добавить проверку пароля
        // if (!VerifyPassword(data.Password, user.PasswordHash))
        // {
        //     throw new ApiException(ApiError.InvalidCredentials);
        // }

        return user;
    }
}
