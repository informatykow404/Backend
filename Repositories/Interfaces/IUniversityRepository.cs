

using Backend.Data.Models;

namespace Backend.Repositories.Interfaces;

public interface IUniversityRepository
{
    Task<University?> GetUniversityByIdAsync(string id, CancellationToken ct = default);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}