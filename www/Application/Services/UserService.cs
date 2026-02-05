using MyMicroservice.Contracts.Requests;
using MyMicroservice.Infrastructure.Repositories;

namespace MyMicroservice.Application.Services;

public interface IUserService
{
    Task<string> RegisterAsync(RegisterJsonRequest data);
}

public class UserService(IUserRepository repository) : IUserService
{
    private readonly IUserRepository _repository = repository;

    public async Task<string> RegisterAsync(RegisterJsonRequest data)
    {

        Console.WriteLine($"Received register request: {data}");

        return await _repository.CreateUser(data);
    }

}
