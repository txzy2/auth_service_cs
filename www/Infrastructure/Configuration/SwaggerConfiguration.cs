using System.Reflection;
using Microsoft.OpenApi;

namespace MyMicroservice.API.Configuration;

public static class SwaggerConfiguration
{
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Auth Microservice API",
                Version = "v1",
                Description = "Authentication and authorization microservice",
                Contact = new OpenApiContact
                {
                    Name = "Your Name",
                    Email = "your.email@example.com"
                }
            });

            // Включаем аннотации
            c.EnableAnnotations();

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            if (File.Exists(xmlPath)) c.IncludeXmlComments(xmlPath);
        });

        return services;
    }

    public static IApplicationBuilder UseSwaggerDocumentation(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Auth API V1");
            c.RoutePrefix = "swagger";
            c.DocumentTitle = "Auth API Documentation";
        });

        var baseUrl = Environment.GetEnvironmentVariable("APP_URL") ?? "http://localhost:4200";
        Console.WriteLine($"=== ✓ Swagger UI available at: {baseUrl}/swagger ===");

        return app;
    }
}