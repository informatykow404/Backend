using Backend.Data.Models;
using Backend.EntityFramework.Contexts;
using Backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Backend.Services.Implementations;

public class RefreshTokenService : IRefreshTokenService
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

    public async Task<RefreshToken?> ValidateRefreshToken(string refreshToken)
    {
        return await _db.RefreshTokens
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Token == refreshToken && r.Valid && r.Expires > DateTime.UtcNow);
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
    
    public async Task RevokeRefreshToken(string refreshToken)
    {
        var token = await _db.RefreshTokens.FirstOrDefaultAsync(r => r.Token == refreshToken);
        if (token != null)
        {
            token.Valid = false;
            _db.RefreshTokens.Update(token);
            await _db.SaveChangesAsync();
        }
    }
    
    public async Task RemoveRefreshToken(string refreshToken)
    {
        var token = await _db.RefreshTokens.FirstOrDefaultAsync(r => r.Token == refreshToken);
        if (token != null)
        {
            _db.RefreshTokens.Remove(token);
            await _db.SaveChangesAsync();
        }
    }
    
    public async Task RemoveAllRefreshTokensForUser(string userId)
    {
        var tokens = await _db.RefreshTokens
            .Where(r => r.User.Id == userId)
            .ToListAsync();

        if (tokens.Count == 0) return;

        _db.RefreshTokens.RemoveRange(tokens);
        await _db.SaveChangesAsync();
    }
}