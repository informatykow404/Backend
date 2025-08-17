using Backend.Data.Models;

namespace Backend.Services.Interfaces
{
    public interface IRefreshTokenService
    {
        Task<string> GenerateRefreshToken(User user);
        Task<RefreshToken?> ValidateRefreshToken(string refreshToken);
        Task RevokeRefreshToken(string refreshToken);
        Task RemoveRefreshToken(string refreshToken);
        Task RemoveAllRefreshTokensForUser(string userId);
    }
}
