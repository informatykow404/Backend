using System.Linq.Expressions;
using Backend.Data.Models;

namespace Backend.Repositories.Interfaces
{
    public interface IScienceClubRepository
    {
        Task<IEnumerable<ScienceClub>> GetAllAsync(CancellationToken ct = default);
        Task<ScienceClub?> GetByIdAsync(string id, CancellationToken ct = default);
        Task AddAsync(ScienceClub club, CancellationToken ct = default);
        void Update(ScienceClub club);
        void Remove(ScienceClub club);
        Task<int> SaveChangesAsync(CancellationToken ct = default);

        Task<TEntity?> FindAsync<TEntity>(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default)
            where TEntity : ScienceClub;
    }
}
