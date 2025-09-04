using Backend.Data.Models;
using Backend.Data.Models.Enums;
using Backend.DTOs.University;
using Backend.EntityFramework.Contexts;
using Backend.Repositories.Implementations;
using Backend.Repositories.Interfaces;
using Backend.Services.Interfaces;

namespace Backend.Services.Implementations;

public class UniversityService : IUniversityService
{
    private readonly IUniversityRepository _universityRepository;
    private readonly IScienceClubRepository _scienceClubRepository;

    public UniversityService(IUniversityRepository universityRepository, IScienceClubRepository scienceClubRepository)
    {
        _universityRepository = universityRepository;
        _scienceClubRepository = scienceClubRepository;
    }
    public async Task<IEnumerable<ScienceClub>> GetPendingClubsAsync(string id, CancellationToken ct = default)
    {
        return await _scienceClubRepository.GetAllPendingClubsAsync(id, ct);
    }
    
    public async Task<(bool,string)> AcceptClubAsync(string id,AcceptDTO approval, CancellationToken ct = default)
    {
        ScienceClub club = await _scienceClubRepository.GetByIdAsync(approval.ClubId, ct);
        if (club == null)
            return (false, "Couldn't find the club with this Id");
        if (approval.Status == true)
            club.status = ClubStatus.Active;
        else club.status = ClubStatus.Inactive;
        University? university = await _universityRepository.GetUniversityByIdAsync(id, ct);
        if (university == null)
            return (false, "University not found");
        ScienceClub? uniClub = university.Clubs.FirstOrDefault(c => c.Id == approval.ClubId);
        if (uniClub == null)
            return (false, "Couldn't find the club with this Id in the university");
        if (approval.Status == true)
            uniClub.status = ClubStatus.Active;
        else uniClub.status = ClubStatus.Inactive;
        await _universityRepository.SaveChangesAsync(ct);
        if (approval.Status == true)
            return (true, "Successfully accepted the club");
        return (true, "Successfully changed the club status to inactive");
    }
    
    public async Task<(bool,string)> CreateUniversityAsync(CreateUniDTO uniData, CancellationToken ct = default)
    {
        University uni = await _universityRepository.GetUniversityByNameAsync(uniData.Name, ct);
        if (uni != null)
            return (false, "University with the given name already exists");
        var university = new University()
        {
            Id = Guid.NewGuid().ToString(),
            Name = uniData.Name!,
            Location = uniData.Location!,
            Description = uniData.Description!,
            Clubs = new List<ScienceClub>()
        };
        await _universityRepository.AddUniversityAsync(university, ct);
        await _universityRepository.SaveChangesAsync(ct);
        return (true, "The university has been created");
    }
}