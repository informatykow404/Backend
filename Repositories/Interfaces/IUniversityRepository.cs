

using Backend.Data.Models;

namespace Backend.Repositories.Interfaces;

public interface IUniversityRepository
{
    Task<University?> GetUniversityByIdAsync(string id, CancellationToken ct = default);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
    Task AddUniversityAsync(University university,CancellationToken ct = default);
    Task<University?> GetUniversityByNameAsync(string name, CancellationToken ct = default);
    void RemoveUniversity(University club);
}