using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Backend.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Services.Implementations;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public JwtSecurityToken GenerateJwtToken(IEnumerable<Claim> claims)
    {
        // PRD
        // var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET");

        // DEV (remove on PRD)
        var jwtSecret = _configuration["JWT_SECRET"];

        var signingKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSecret));

        return new JwtSecurityToken(
            issuer: _configuration["Authentication:ValidIssuer"],
            audience: _configuration["Authentication:ValidAudience"],
            expires: DateTime.UtcNow.AddHours(3),
            claims: claims,
            signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
        );
    }
}