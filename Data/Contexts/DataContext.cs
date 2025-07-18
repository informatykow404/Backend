using Backend.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Backend.EntityFramework.Contexts;

public class DataContext : IdentityDbContext<User>
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<ScienceClub> ScienceClubs { get; set; }
    
    public DbSet<RefreshToken> RefreshTokens { get; set; }
}