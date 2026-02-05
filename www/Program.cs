using DotNetEnv;
using MyMicroservice.API.Configuration;
using MyMicroservice.Application.Configuration;
using MyMicroservice.Infrastructure.Configuration;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Конфигурация сервисов
builder.Services.AddDatabase();
builder.Services.AddRepositories();
builder.Services.AddApplicationServices();
builder.Services.AddControllers();
builder.Services.AddSwaggerDocumentation();

var app = builder.Build();

// Валидация подключения к БД
await app.ValidateDatabaseConnectionAsync();

// Конфигурация middleware
app.UseSwaggerDocumentation();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();