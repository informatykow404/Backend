using Backend.Data.Models;
using Backend.Data.Models.Enums;
using Backend.DTOs.ScienceClub;
using Backend.Repositories.Interfaces;
using Backend.Services.Interfaces;

namespace Backend.Services.Implementations
{
    public class ScienceClubService : IScienceClubService
    {
        private readonly IScienceClubRepository _scienceClubRepository;
        private readonly IUserRepository _userRepository;
        public ScienceClubService(IScienceClubRepository scienceClubRepository, IUserRepository userRepository)
        {
            _scienceClubRepository = scienceClubRepository;
            _userRepository = userRepository;
        }

        public async Task<(bool, string)> CreateClubAsync(CreateDTO club, string userName, CancellationToken ct = default)
        {
            var clubName = await _scienceClubRepository.FindAsync<ScienceClub>(c => c.Name == club.Name, ct);
            var user = await _userRepository.GetByUsernameAsync(userName, ct);
            if (clubName != null) return (false, "The club with this name already exists.");
            if (user != null) return (false, "Could not find user.");
            var scienceClub = new ScienceClub()
            {
                Id = Guid.NewGuid().ToString(),
                Name = club.Name!,
                Users = new List<User> { user! },
                status  = ClubStatus.Pending
            };
            var clubMember = new ClubMember()
            {
                Id = Guid.NewGuid().ToString(),
                User = user!,
                Club = scienceClub,
                Role = ScienceClubRole.President
            };
            var university = new University()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "universityName",
                Location = "",
                Description = "",
                Clubs = new List<ScienceClub> { scienceClub }
            };
            await _scienceClubRepository.AddClubAsync(scienceClub, clubMember, university, ct);
            await _scienceClubRepository.SaveChangesAsync(ct);
            return (true, "The club is waiting for approval.");
        }
        
        public async Task<(bool, string)> JoinClubAsync(string id, string userName, CancellationToken ct = default)
        {
            var user = await _userRepository.GetByUsernameAsync(userName, ct);
            if (user != null) return (false, "Could not find user.");
            var club = await _scienceClubRepository.FindAsync<ScienceClub>(c => c.Id == id, ct);
            if (club != null) return (false, "Could not find club.");
            club!.Users!.Add(user!);
            var clubMember = new ClubMember()
            {
                Id = Guid.NewGuid().ToString(),
                User = user!,
                Club = club,
                Role = ScienceClubRole.User
            };
            await _scienceClubRepository.JoinClubAsync(clubMember, ct);
            _scienceClubRepository.Update(club);
            await _scienceClubRepository.SaveChangesAsync(ct);
            return (true, "You successfully joined the club.");
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
