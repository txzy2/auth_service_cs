using System.Text.Json;
using StackExchange.Redis;

namespace MyMicroservice.Application.Services;

public interface IRedisCacheService
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
    Task<bool> DeleteAsync(string key);
    Task<bool> ExistsAsync(string key);
}

public class RedisCacheService(IDatabase database) : IRedisCacheService
{
    private readonly IDatabase _database = database;

    public async Task<T?> GetAsync<T>(string key)
    {
        var value = await _database.StringGetAsync(key);

        return value.IsNullOrEmpty
            ? default
            :
            // Явное преобразование RedisValue в string
            JsonSerializer.Deserialize<T>(value.ToString());
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var serializedValue = JsonSerializer.Serialize(value);
        await _database.StringSetAsync(
            key,
            serializedValue,
            expiry.HasValue ? new Expiration(expiry.Value) : Expiration.Default
        );
    }

    public async Task<bool> DeleteAsync(string key)
    {
        return await _database.KeyDeleteAsync(key);
    }

    public async Task<bool> ExistsAsync(string key)
    {
        return await _database.KeyExistsAsync(key);
    }
}