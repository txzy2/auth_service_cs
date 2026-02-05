using System.Reflection;

namespace MyMicroservice.API.Configuration;

public static class SwaggerConfiguration
{
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new()
            {
                Title = "Auth Microservice API",
                Version = "v1",
                Description = "Authentication and authorization microservice"
            });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
            }
        });

        return services;
    }

    public static IApplicationBuilder UseSwaggerDocumentation(this WebApplication app)
    {
        // Убрали проверку на IsDevelopment - теперь работает всегда
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Auth API V1");
            c.RoutePrefix = "swagger";
        });

        var baseUrl = Environment.GetEnvironmentVariable("APP_URL") ?? "Set Base url in .env file";
        Console.WriteLine($"✓ Swagger UI available at: {baseUrl}/swagger");
        Console.WriteLine($"✓ Swagger JSON available at: {baseUrl}/swagger/v1/swagger.json");

        return app;
    }
}