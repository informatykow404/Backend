using Backend.Data.Models;
using Backend.Repositories.Interfaces;
using Backend.Services.Interfaces;

namespace Backend.Services.Implementations
{
    public class ScienceClubService : IScienceClubService
    {
        private readonly IScienceClubRepository _scienceClubRepository;
        public ScienceClubService(IScienceClubRepository scienceClubRepository)
        {
            _scienceClubRepository = scienceClubRepository;
        }

        public async Task<ScienceClub> CreateAsync(ScienceClub club, CancellationToken ct = default)
        {
            await _scienceClubRepository.AddAsync(club, ct);
            await _scienceClubRepository.SaveChangesAsync(ct);
            return club;
        }

        public async Task<bool> DeleteAsync(string  id, CancellationToken ct = default)
        {
            var existing = await _scienceClubRepository.GetByIdAsync(id, ct);
            if (existing is null) return false;

            _scienceClubRepository.Remove(existing);
            await _scienceClubRepository.SaveChangesAsync(ct);
            return true;
        }

        public async Task<IEnumerable<ScienceClub>> GetAllAsync(CancellationToken ct = default)
        {
            return await _scienceClubRepository.GetAllAsync(ct);
        }

        public async Task<ScienceClub?> GetByIdAsync(string id, CancellationToken ct = default)
        {
            return await _scienceClubRepository.GetByIdAsync(id, ct);
        }

        public async Task<bool> UpdateAsync(string id, ScienceClub club, CancellationToken ct = default)
        {
            var existing = await _scienceClubRepository.GetByIdAsync(id, ct);
            if (existing is null) return false;

            existing.Name = club.Name;
            existing.Users = club.Users;

            _scienceClubRepository.Update(existing);
            await _scienceClubRepository.SaveChangesAsync(ct);
            return true;
        }
    }
}
