using System.IdentityModel.Tokens.Jwt;
using Backend.Data.Models;
using Backend.DTOs.Auth;

namespace Backend.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllAsync(CancellationToken ct = default);
        Task<User?> GetByGuidAsync(string guid, CancellationToken ct = default);
        Task<User> CreateAsync(User user, CancellationToken ct = default);
        Task<bool> UpdateAsync(string guid, User user, CancellationToken ct = default);
        Task<bool> DeleteAsync(string guid, CancellationToken ct = default);
        Task<(bool, string, JwtSecurityToken)> ReplaceData(DataUpdateDTO data, string username);
        Task<GetInfoAboutUser?> GetDataAboutUser(string username, CancellationToken ct = default);
    }
}
