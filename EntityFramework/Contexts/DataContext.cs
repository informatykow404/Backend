using System.Text.Json;
using Backend.EntityFramework.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Backend.EntityFramework.Contexts;

public class DataContext : IdentityDbContext<User>
{
    public DbSet<User> Users { get; set; }
    public DbSet<ScienceClub> ScienceClubs { get; set; }

    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }
}