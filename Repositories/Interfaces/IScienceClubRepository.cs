using Backend.Data.Models;

namespace Backend.Repositories.Interfaces
{
    public interface IScienceClubRepository
    {
        Task<IEnumerable<ScienceClub>> GetAllAsync(CancellationToken ct = default);
        Task<ScienceClub?> GetByIdAsync(int id, CancellationToken ct = default);
        Task AddAsync(ScienceClub club, CancellationToken ct = default);
        void Update(ScienceClub club);
        void Remove(ScienceClub club);
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}
