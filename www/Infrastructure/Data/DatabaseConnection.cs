using Npgsql;

namespace MyMicroservice.Infrastructure.Data;

public interface IDatabaseConnection
{
    NpgsqlConnection CreateConnection();
    Task<bool> TestConnectionAsync();
}

public class DatabaseConnection(string connectionString) : IDatabaseConnection
{
    private readonly string _connectionString = connectionString;

    public NpgsqlConnection CreateConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }

    public async Task<bool> TestConnectionAsync()
    {
        try
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            // Опционально: проверка версии БД
            await using var cmd = new NpgsqlCommand("SELECT version()", connection);
            var version = await cmd.ExecuteScalarAsync();

            Console.WriteLine($"✓ Database connection successful.");
            Console.WriteLine($"  PostgreSQL version: {version}");

            return true;
        }
        catch (NpgsqlException ex)
        {
            Console.WriteLine($"✗ Database connection error: {ex.Message}");
            Console.WriteLine($"  Error code: {ex.ErrorCode}");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ Unexpected error: {ex.Message}");
            return false;
        }
    }
}