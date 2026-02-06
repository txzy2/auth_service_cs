using Microsoft.EntityFrameworkCore;
using MyMicroservice.Contracts;
using MyMicroservice.Contracts.Responses;
using MyMicroservice.Domain.Entities;
using MyMicroservice.Enums;
using MyMicroservice.Infrastructure.Data;

namespace MyMicroservice.Infrastructure.Repositories;

public interface IUserRepository
{
    Task<UserJsonResponse?> CreateUser(PreparedRegisterJsonRequest data);
    Task<User?> GetUserByEmailAsync(string email);
    Task<User?> GetUserByLoginAsync(string login);
}

public class UserRepository(ApplicationDbContext _context, ILogger<UserRepository> logger) : IUserRepository
{
    private readonly ApplicationDbContext _context = _context;
    private readonly ILogger<UserRepository> _logger = logger;

    public async Task<UserJsonResponse?> CreateUser(PreparedRegisterJsonRequest data)
    {
        Console.WriteLine($"User Repo works. User saved: {data}");
        _logger.LogInformation($"role {data.Role.ToRoleId()}");
        try
        {
            var user = new User
            {
                Name = data.Name,
                Email = data.Email,
                Login = data.Login,
                PasswordHash = data.PasswordHash,
                ExtId = data.ExtId,
                RoleId = data.Role.ToRoleId()
            };
            
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserJsonResponse(
                Login: user.Login,
                Email: user.Email,
                Name: user.Name,
                Role: data.Role.ToAlias(),
                CreatedAt: user.CreatedAt,
                UpdatedAt: user.UpdatedAt ?? DateTime.UtcNow
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating user: {ex.Message}");
            return null;
        }
       
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetUserByLoginAsync(string login)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Login == login);
    }
}
