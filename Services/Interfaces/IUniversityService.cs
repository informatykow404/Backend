using Backend.Data.Models;
using Backend.DTOs.University;

namespace Backend.Services.Interfaces;

public interface IUniversityService
{ 
    Task<IEnumerable<ScienceClub>> GetPendingClubsAsync(string id, CancellationToken ct = default);
    Task<(bool,string)> AcceptClubAsync(string id, AcceptDTO approval, CancellationToken ct = default);
    Task<(bool,string)> CreateUniversityAsync(CreateUniDTO uniData, CancellationToken ct = default);
}