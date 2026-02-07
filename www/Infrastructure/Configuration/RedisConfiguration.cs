using StackExchange.Redis;

namespace MyMicroservice.Infrastructure.Configuration;

public static class RedisConfiguration
{
    public static IServiceCollection AddRedis(this IServiceCollection services)
    {
        var redisConnectionString = Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING")
                                    ?? "localhost:6379";

        var configurationOptions = ConfigurationOptions.Parse(redisConnectionString);
        configurationOptions.AbortOnConnectFail = false;
        configurationOptions.ConnectTimeout = 5000;
        configurationOptions.SyncTimeout = 5000;
        configurationOptions.AsyncTimeout = 5000;
        configurationOptions.ConnectRetry = 3;

        // Singleton для ConnectionMultiplexer (важно!)
        services.AddSingleton<IConnectionMultiplexer>(sp => ConnectionMultiplexer.Connect(configurationOptions));

        // Scoped для IDatabase
        services.AddScoped<IDatabase>(sp =>
        {
            var multiplexer = sp.GetRequiredService<IConnectionMultiplexer>();
            return multiplexer.GetDatabase();
        });

        return services;
    }
}

public static class RedisValidationExtensions
{
    public static async Task ValidateRedisConnectionAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            var redis = scope.ServiceProvider.GetRequiredService<IConnectionMultiplexer>();

            var database = redis.GetDatabase();
            await database.PingAsync();

            logger.LogInformation("=== ✅ Redis connection successful ===");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "❌ Redis connection failed");
            throw;
        }
    }
}