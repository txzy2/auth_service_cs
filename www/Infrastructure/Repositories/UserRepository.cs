using Microsoft.EntityFrameworkCore;
using MyMicroservice.Contracts.Requests;
using MyMicroservice.Domain.Entities;
using MyMicroservice.Infrastructure.Data;

namespace MyMicroservice.Infrastructure.Repositories;

public interface IUserRepository
{
    Task<string> CreateUser(RegisterJsonRequest data);
    Task<User?> GetUserByEmailAsync(string email);
}

public class UserRepository(ApplicationDbContext _context) : IUserRepository
{
    private readonly ApplicationDbContext _context = _context;

    public async Task<string> CreateUser(RegisterJsonRequest data)
    {
        Console.WriteLine($"User Repo works. User saved: {data}");

        await Task.CompletedTask;
        return $"User {data.Login} saved successfully";
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }
}
