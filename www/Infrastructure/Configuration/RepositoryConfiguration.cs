using MyMicroservice.Infrastructure.Repositories;

namespace MyMicroservice.Infrastructure.Configuration;

public static class RepositoryConfiguration
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRolesRepository, RolesRepository>();

        return services;
    }
    
}