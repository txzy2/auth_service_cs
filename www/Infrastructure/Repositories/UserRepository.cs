using MyMicroservice.Contracts.Requests;
using MyMicroservice.Infrastructure.Data;

namespace MyMicroservice.Infrastructure.Repositories;

public interface IUserRepository
{
    Task<string> CreateUser(RegisterJsonRequest data);
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
}
