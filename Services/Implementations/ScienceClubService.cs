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
            if (clubName == null) return (false, "The club with this name already exists.");
            if (user == null) return (false, "Could not find user.");
            var scienceClub = new ScienceClub()
            {
                Id = Guid.NewGuid().ToString(),
                Name = club.Name!,
                status  = ClubStatus.Pending,
                UniversityId = club.UniversityId!,
            };
            var clubMember = new ClubMember()
            {
                Id = Guid.NewGuid().ToString(),
                User = user,
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
            if (user == null) return (false, "Could not find user.");
            var club = await _scienceClubRepository.FindAsync<ScienceClub>(c => c.Id == id, ct);
            if (club == null) return (false, "Could not find club.");

            var clubMember = new ClubMember()
            {
                Id = Guid.NewGuid().ToString(),
                User = user,
                Club = club,
                Role = ScienceClubRole.User
            };
            await _scienceClubRepository.JoinClubAsync(clubMember, ct);
            _scienceClubRepository.UpdateScienceClub(club);
            await _scienceClubRepository.SaveChangesAsync(ct);
            return (true, "You have successfully joined the club.");
        }
        
        public async Task<(bool, string)> GetUsersAsync(string id, string userName, CancellationToken ct = default)
        {
            // TODO correct this method
            var user = await _userRepository.GetByUsernameAsync(userName, ct);
            if (user == null) return (false, "Could not find user.")!;
            var clubMemberData = await _scienceClubRepository.GetClubMemberByUserAsync(user, id, ct);
            if (clubMemberData == null) return (false, "Could not find user in this club.")!;
            if (clubMemberData.Role != ScienceClubRole.President && clubMemberData.Role != ScienceClubRole.Admin) return (false, "")!;
            var club = await _scienceClubRepository.FindAsync<ScienceClub>(c => c.Id == id, ct);
            if (club == null) return (false, "Could not find club.")!;
            return (true, "")!;
        }
        
        public async Task<(bool, string)> ModifyUserRoleAsync(string clubId, string userId, ScienceClubRole role, string userName, CancellationToken ct = default)
        {
            var user = await _userRepository.GetByUsernameAsync(userName, ct);
            if (user == null) return (false, "Could not find user.");
            var clubMemberData = await _scienceClubRepository.GetClubMemberByUserAsync(user, clubId, ct);
            if (clubMemberData == null) return (false, "Could not find user in this club.");
            if (clubMemberData.Role != ScienceClubRole.President && clubMemberData.Role != ScienceClubRole.Admin) return (false, "");
            var clubMemberToUpdate = await _scienceClubRepository.GetClubMemberByIdAsync(userId, clubId, ct);
            if (clubMemberToUpdate == null) return (false, "Could not find club member in this club.");
            if(clubMemberData.Role == ScienceClubRole.Admin && (role == ScienceClubRole.President || clubMemberToUpdate.Role == ScienceClubRole.President)) return (false, "");
            clubMemberToUpdate.Role = role;
            _scienceClubRepository.UpdateClubMember(clubMemberToUpdate);
            await _scienceClubRepository.SaveChangesAsync(ct);
            return (true, "You have successfully updated the user role.");
        }
        
        public async Task<(bool, string)> ModifyScienceClubAsync(string clubId, DescriptionDTO description, string userName, CancellationToken ct = default)
        {
            var user = await _userRepository.GetByUsernameAsync(userName, ct);
            if (user == null) return (false, "Could not find user.");
            var clubMemberData = await _scienceClubRepository.GetClubMemberByUserAsync(user, clubId, ct);
            if (clubMemberData == null) return (false, "Could not find user in this club.");
            if (clubMemberData.Role != ScienceClubRole.President && clubMemberData.Role != ScienceClubRole.Admin) return (false, "");
            var scienceClub = await _scienceClubRepository.GetByIdAsync(clubId, ct);
            if (scienceClub == null) return (false, "Could not find club.");
            scienceClub.Name = description.Name;
            scienceClub.Description = description.Description;
            _scienceClubRepository.UpdateScienceClub(scienceClub);
            await _scienceClubRepository.SaveChangesAsync(ct);
            return (true, "You have successfully modified the club.");
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

            _scienceClubRepository.UpdateScienceClub(existing);
            await _scienceClubRepository.SaveChangesAsync(ct);
            return true;
        }
    }
    
}
