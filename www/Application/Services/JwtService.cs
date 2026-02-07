using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace MyMicroservice.Application.Services;

public interface IJwtService
{
    string GenerateToken(string userId, string login, string email, string role);
    ClaimsPrincipal? ValidateToken(string token);
}

public class JwtService(IConfiguration configuration, ILogger<JwtService> logger) : IJwtService
{
    private readonly string _audience = configuration["Jwt:Audience"] ?? "MyMicroserviceUsers";
    private readonly int _expirationMinutes = int.Parse(configuration["Jwt:ExpirationMinutes"] ?? "30");
    private readonly string _issuer = configuration["Jwt:Issuer"] ?? "MyMicroservice";

    private readonly string _secret = configuration["Jwt:Secret"]
                                      ?? throw new InvalidOperationException("JWT Secret not configured");

    public string GenerateToken(string userId, string login, string email, string role)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("login", login),
            new Claim(ClaimTypes.Role, role)
        };

        logger.LogInformation("SECRET: {SECRET}", _secret);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            _issuer,
            _audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(_expirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_secret);

        try
        {
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _issuer,
                ValidAudience = _audience,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero
            }, out _);

            return principal;
        }
        catch
        {
            return null;
        }
    }
}