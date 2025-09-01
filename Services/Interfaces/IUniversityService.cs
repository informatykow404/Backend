using Backend.Data.Models;

namespace Backend.Services.Interfaces;

public interface IUniversityService
{ 
    Task<IEnumerable<ScienceClub>> GetPendingClubsAsync(string id, CancellationToken ct = default);
}