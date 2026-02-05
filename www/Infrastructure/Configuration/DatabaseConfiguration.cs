using Microsoft.EntityFrameworkCore;
using MyMicroservice.Infrastructure.Data;

namespace MyMicroservice.Infrastructure.Configuration;

public static class DatabaseConfiguration
{
    public static IServiceCollection AddDatabase(this IServiceCollection services)
    {
        // Валидация переменных окружения
        var requiredEnvVars = new[] { "DB_HOST", "DB_PORT", "DB_NAME", "DB_USER", "DB_PASS" };
        var missingVars = requiredEnvVars
            .Where(v => string.IsNullOrEmpty(Environment.GetEnvironmentVariable(v)))
            .ToList();

        if (missingVars.Count != 0)
        {
            throw new InvalidOperationException(
                $"Missing required environment variables: {string.Join(", ", missingVars)}");
        }

        var connectionString = BuildConnectionString();

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorCodesToAdd: null);
            })
        );


        var safeConnectionString = connectionString.Replace(
            Environment.GetEnvironmentVariable("DB_PASS")!, "***");
        Console.WriteLine($"Connection String: {safeConnectionString}");

        services.AddSingleton<IDatabaseConnection>(new DatabaseConnection(connectionString));

        return services;
    }

    public static async Task ValidateDatabaseConnectionAsync(this WebApplication app)
    {
        var databaseConnection = app.Services.GetRequiredService<IDatabaseConnection>();

        if (!await databaseConnection.TestConnectionAsync())
        {
            throw new InvalidOperationException(
                "Failed to connect to the database. Please check your connection settings.");
        }
    }

    private static string BuildConnectionString()
    {
        return $"Host={Environment.GetEnvironmentVariable("DB_HOST")};" +
               $"Port={Environment.GetEnvironmentVariable("DB_PORT")};" +
               $"Database={Environment.GetEnvironmentVariable("DB_NAME")};" +
               $"Username={Environment.GetEnvironmentVariable("DB_USER")};" +
               $"Password={Environment.GetEnvironmentVariable("DB_PASS")}";
    }

}