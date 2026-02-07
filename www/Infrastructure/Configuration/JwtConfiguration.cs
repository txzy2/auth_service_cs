using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MyMicroservice.Application.Services;

namespace MyMicroservice.Infrastructure.Configuration;

public static class JwtConfiguration
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services,
        IConfiguration configuration)
    {
        var secret = configuration["Jwt:Secret"]
                     ?? throw new InvalidOperationException("JWT Secret not configured");
        var issuer = configuration["Jwt:Issuer"] ?? "MyMicroservice";
        var audience = configuration["Jwt:Audience"] ?? "MyMicroserviceUsers";

        services.AddScoped<IJwtService, JwtService>();

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                    ClockSkew = TimeSpan.Zero
                };
            });

        return services;
    }
}