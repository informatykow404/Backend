using Backend.Data.Models;
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
}