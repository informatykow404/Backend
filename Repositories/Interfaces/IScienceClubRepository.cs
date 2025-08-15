using System.Linq.Expressions;
using Backend.Data.Models;

namespace Backend.Repositories.Interfaces
{
    public interface IScienceClubRepository
    {
        Task<IEnumerable<ScienceClub>> GetAllAsync(CancellationToken ct = default);
        Task<ScienceClub?> GetByIdAsync(string id, CancellationToken ct = default);
        Task AddClubAsync(ScienceClub scienceClub, ClubMember clubMember, University university,CancellationToken ct = default);
        Task JoinClubAsync(ClubMember clubMember, CancellationToken ct = default);
        Task<ClubMember?> GetClubMemberByUserAsync(User user, string clubId, CancellationToken ct = default);
        Task<ClubMember?> GetClubMemberByIdAsync(string userId, string clubId, CancellationToken ct = default);
        void UpdateScienceClub(ScienceClub club);
        void UpdateClubMember(ClubMember club);
        void Remove(ScienceClub club);
        Task<int> SaveChangesAsync(CancellationToken ct = default);

        Task<TEntity?> FindAsync<TEntity>(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default)
            where TEntity : ScienceClub;
    }
}
