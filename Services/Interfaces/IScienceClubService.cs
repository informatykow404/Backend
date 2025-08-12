using Backend.Data.Models;
using Backend.DTOs.ScienceClub;

namespace Backend.Services.Interfaces
{
    public interface IScienceClubService
    {
        Task<IEnumerable<ScienceClub>> GetAllAsync(CancellationToken ct = default);
        Task<ScienceClub?> GetByIdAsync(string id, CancellationToken ct = default);
        Task<(bool, string)> CreateClubAsync(CreateDTO club, string userName, CancellationToken ct = default);
        Task<(bool, string)> JoinClubAsync(string id, string userName, CancellationToken ct = default);
        Task<bool> UpdateAsync(string id, ScienceClub club, CancellationToken ct = default);
        Task<bool> DeleteAsync(string id, CancellationToken ct = default);
    }
}
