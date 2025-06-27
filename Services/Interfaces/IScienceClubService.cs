using Backend.Data.Models;

namespace Backend.Services.Interfaces
{
    public interface IScienceClubService
    {
        Task<IEnumerable<ScienceClub>> GetAllAsync(CancellationToken ct = default);
        Task<ScienceClub?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<ScienceClub> CreateAsync(ScienceClub club, CancellationToken ct = default);
        Task<bool> UpdateAsync(int id, ScienceClub club, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
