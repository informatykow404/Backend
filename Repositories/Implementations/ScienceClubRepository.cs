using System.Linq.Expressions;
using Backend.Data.Models;
using Backend.EntityFramework.Contexts;
using Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories.Implementations
{
    public class ScienceClubRepository : IScienceClubRepository
    {
        private readonly DataContext _context;
        public ScienceClubRepository(DataContext context) => _context = context;

        public async Task AddClubAsync(ScienceClub scienceClub, ClubMember clubMember, University university,CancellationToken ct = default)
        {
            await _context.ScienceClubs.AddAsync(scienceClub, ct);
            await _context.ClubMembers.AddAsync(clubMember, ct);
            await _context.Universities.AddAsync(university, ct);
        }
        
        public async Task JoinClubAsync(ClubMember clubMember, CancellationToken ct = default)
        {
            await _context.ClubMembers.AddAsync(clubMember, ct);
        }

        public async Task<IEnumerable<ScienceClub>> GetAllAsync(CancellationToken ct = default)
        {
            return await _context.ScienceClubs
                .AsNoTracking()
                .ToListAsync(ct);
        }

        public async Task<ScienceClub?> GetByIdAsync(string id, CancellationToken ct = default)
        {
            return await _context.ScienceClubs
                .FirstOrDefaultAsync(c => c.Id == id, ct);
        }

        public void Remove(ScienceClub club)
        {
            _context.Remove(club);
        }

        public void UpdateScienceClub(ScienceClub club)
        {
           _context.ScienceClubs.Update(club);
        }
        
        public void UpdateClubMember(ClubMember club)
        {
            _context.ClubMembers.Update(club);
        }

        public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            return await _context.SaveChangesAsync(ct);
        }

        public async Task<TEntity?> FindAsync<TEntity>(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default) where TEntity : ScienceClub
        {
            return await _context.Set<TEntity>().FirstOrDefaultAsync(predicate, ct);
        }

        public async Task<ClubMember?> GetClubMemberByUserAsync(User user, string clubId, CancellationToken ct = default)
        {
            return await _context.ClubMembers.FirstOrDefaultAsync(c => c.User == user && c.Club.Id == clubId, ct);
        }
        
        public async Task<ClubMember?> GetClubMemberByIdAsync(string id, string clubId, CancellationToken ct = default)
        {
            return await _context.ClubMembers.FirstOrDefaultAsync(c => c.Id == id && c.Club.Id == clubId, ct);
            
        }
    }
}
