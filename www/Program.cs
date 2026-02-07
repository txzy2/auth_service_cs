using DotNetEnv;
using MyMicroservice.API.Configuration;
using MyMicroservice.Application.Configuration;
using MyMicroservice.Infrastructure.Configuration;
using MyMicroservice.Middleware;
using Serilog;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();
builder.Configuration["Jwt:Secret"] = Environment.GetEnvironmentVariable("JWT_SECRET");

// Конфигурация сервисов
builder.Services.AddDatabase();
builder.Services.AddRedis();
builder.Services.AddRepositories();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddControllers();
builder.Services.AddSwaggerDocumentation();


builder.Host.UseSerilog((context, configuration) =>
{
    var logPath = Path.Combine("logs", DateTime.Now.ToString("yyyy"), DateTime.Now.ToString("MM"), "info.log");

    configuration
        .ReadFrom.Configuration(context.Configuration)
        .WriteTo.Console()
        .WriteTo.File(
            logPath,
            rollingInterval: RollingInterval.Day,
            rollOnFileSizeLimit: true,
            retainedFileCountLimit: 31,
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
        .Enrich.FromLogContext();
});

var app = builder.Build();

await app.ValidateDatabaseConnectionAsync();
await app.ValidateRedisConnectionAsync();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwaggerDocumentation();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();