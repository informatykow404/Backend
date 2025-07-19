using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Backend.Data.Models;
using Backend.EntityFramework.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Services.Implementations;

public class RefreshTokenService
{
    
    private readonly DataContext _db;
    private readonly IConfiguration _configuration;
    public RefreshTokenService(DataContext db, IConfiguration configuration)
    {
        _db = db;
        _configuration = configuration;
    }
    //main function for generating refresh token
    public async Task<string> GenerateRefreshToken(User user)
    {
        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid().ToString(),
            User = user,
            Token = GenerateSecureToken(),
            Expires = DateTime.UtcNow.AddDays(GetRefreshTokenLifetimeDays()),
            Valid = true
        };
        _db.RefreshTokens.Add(refreshToken);
        await _db.SaveChangesAsync();
        
        return refreshToken.Token;
    }
//function for generating secure token by generating and combining three GUID and hashing then converting hash back to GUID form
    private string GenerateSecureToken()
    {
        var guid1 = Guid.NewGuid();
        var guid2 = Guid.NewGuid();
        var guid3 = Guid.NewGuid();
        
        var combined=$"{guid1}{guid2}{guid3}";
        using var sha256 = SHA256.Create();
        var hash=sha256.ComputeHash(Encoding.UTF8.GetBytes(combined));

        var guidBytes = new byte[16];
        Array.Copy(hash,0,guidBytes,0,16);
        
        return new Guid(guidBytes).ToString();
    }
    //function which sets life of token to certain value from appsetings.json, and if no value is found there it sets it to 7 days
    private int GetRefreshTokenLifetimeDays()
    {
        return _configuration.GetValue<int>("JwtSettings:RefreshTokenLifetimeDays", 7);
    }
  
}