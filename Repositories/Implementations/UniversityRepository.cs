
using Backend.Data.Models;
using Backend.EntityFramework.Contexts;
using Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories.Implementations;

public class UniversityRepository : IUniversityRepository
{
    private readonly DataContext _context;

    public UniversityRepository(DataContext context)
    {
        _context = context;
    }
    public async Task<University?> GetUniversityByIdAsync(string id, CancellationToken ct = default)
    {
        return await _context.Universities
            .Include(u => u.Clubs)
            .FirstOrDefaultAsync(c => c.Id == id, ct);
    }
    
    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        return await _context.SaveChangesAsync(ct);
    }
    
    public async Task AddUniversityAsync( University university,CancellationToken ct = default)
    {
        await _context.Universities.AddAsync(university, ct);
    }
    
    public async Task<University?> GetUniversityByNameAsync(string name, CancellationToken ct = default)
    {
        return await _context.Universities
            .Include(u => u.Clubs)
            .FirstOrDefaultAsync(c => c.Name == name, ct);
    }
    
    public void RemoveUniversity(University club)
    {
        _context.Remove(club);
    }
}