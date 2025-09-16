using System.Linq.Expressions;
using Backend.Data.Models;
using Backend.Data.Models.Enums;
using Backend.EntityFramework.Contexts;
using Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories.Implementations
{
    public class ScienceClubRepository : IScienceClubRepository
    {
        private readonly DataContext _context;
        public ScienceClubRepository(DataContext context) => _context = context;

        public async Task AddClubAsync(ScienceClub scienceClub, ClubMember clubMember,CancellationToken ct = default)
        {
            await _context.ScienceClubs.AddAsync(scienceClub, ct);
            await _context.ClubMembers.AddAsync(clubMember, ct);
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
        
        public async Task<IEnumerable<ScienceClub>> GetAllPendingClubsAsync(string universityId, CancellationToken ct = default)
        {
            return await _context.ScienceClubs
                .AsNoTracking()
                .Where(c => c.status == ClubStatus.Pending && c.UniversityId == universityId)
                .ToListAsync(ct); 
        }

        public async Task<ScienceClub?> GetByIdAsync(string id, CancellationToken ct = default)
        {
            return await _context.ScienceClubs
                .FirstOrDefaultAsync(c => c.Id == id, ct);
        }

        public async Task<IEnumerable<ScienceClub>> GetActiveClubsByUniversityId(string universityId, CancellationToken ct = default)
        {
            return await _context.ScienceClubs
                .Where(sc => sc.UniversityId == universityId && sc.status == ClubStatus.Active)
                .ToListAsync(ct);
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

        public async Task<ScienceClub?> GetByNameAsync(string name, CancellationToken ct = default)
        {
            return await _context.ScienceClubs
                .FirstOrDefaultAsync(c => c.Name == name, ct);
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
