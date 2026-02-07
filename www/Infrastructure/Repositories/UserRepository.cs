using Microsoft.EntityFrameworkCore;
using MyMicroservice.Contracts.Responses;
using MyMicroservice.Domain.Entities;
using MyMicroservice.Domain.ValueObjects;
using MyMicroservice.Enums;
using MyMicroservice.Infrastructure.Data;

namespace MyMicroservice.Infrastructure.Repositories;

public interface IUserRepository
{
    Task<UserJsonResponse> CreateUser(PreparedRegisterJsonRequest data);
    Task<User?> GetUserByEmailAsync(string email);
    Task<User?> GetUserByLoginAsync(string login);
}

public class UserRepository(ApplicationDbContext context, ILogger<UserRepository> logger) : IUserRepository
{
    public async Task<UserJsonResponse> CreateUser(PreparedRegisterJsonRequest data)
    {
        logger.LogInformation("Creating user {Login} with role {RoleId}", data.Login, data.Role.ToRoleId());
        var user = new User
        {
            Name = data.Name,
            Email = data.Email,
            Login = data.Login,
            PasswordHash = data.PasswordHash,
            ExtId = data.ExtId,
            RoleId = data.Role.ToRoleId()
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        return new UserJsonResponse(
            user.Login,
            user.Email,
            user.Name,
            data.Role.ToAlias(),
            user.CreatedAt,
            user.UpdatedAt ?? DateTime.UtcNow
        );
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetUserByLoginAsync(string login)
    {
        return await context.Users.FirstOrDefaultAsync(u => u.Login == login);
    }
}