using Backend.Data.Models;
using Backend.EntityFramework.Contexts;
using Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        public UserRepository(DataContext context) => _context = context;
       
        public async Task AddAsync(User user, CancellationToken ct = default)
        {
            await _context.Users.AddAsync(user, ct);
        }

        public async Task<IEnumerable<User>> GetAllAsync(CancellationToken ct = default)
        {
            return await _context.Users
                .AsNoTracking()
                .ToListAsync(ct);
        }

        public async Task<User?> GetByGuidAsync(string guid, CancellationToken ct = default)
        {
            return await _context.Users
                .FirstOrDefaultAsync(c => c.Id == guid, ct);
        }

        public void Remove(User user)
        {
            _context.Remove(user);
        }

        public void Update(User user)
        {
            _context.Users.Update(user);
        }

        public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            return await _context.SaveChangesAsync(ct);
        }

        public async Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default)
        {
            return await _context.Users.Where(u=>u.UserName == username).FirstOrDefaultAsync(ct);
        }
    }
}
