using DotNetEnv;
using MyMicroservice.API.Configuration;
using MyMicroservice.Application.Configuration;
using MyMicroservice.Infrastructure.Configuration;
using Serilog;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Конфигурация сервисов
builder.Services.AddDatabase();
builder.Services.AddRepositories();
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
            path: logPath,
            rollingInterval: RollingInterval.Day,
            rollOnFileSizeLimit: true,
            retainedFileCountLimit: 31,
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
        .Enrich.FromLogContext();
});

var app = builder.Build();

// Валидация подключения к БД
await app.ValidateDatabaseConnectionAsync();

// Конфигурация middleware
app.UseSwaggerDocumentation();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();