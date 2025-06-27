using Backend.Data.Models;

namespace Backend.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllAsync(CancellationToken ct = default);
        Task<User?> GetByGuidAsync(string guid, CancellationToken ct = default);
        Task<User> CreateAsync(User user, CancellationToken ct = default);
        Task<bool> UpdateAsync(string guid, User user, CancellationToken ct = default);
        Task<bool> DeleteAsync(string guid, CancellationToken ct = default);
    }
}
