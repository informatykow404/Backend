using System.Text.Json;
using Backend.EntityFramework.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Backend.EntityFramework.Contexts;

public class DataContext : IdentityDbContext<User>
{
    public DbSet<User> Users { get; set; }
    public DbSet<ScienceClub> ScienceClubs { get; set; }
    public DbSet<ClubMember> ClubMembers { get; set; }
    public DbSet<University> Universities { get; set; }

    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<ScienceClub>().ToTable("ScienceClubs", "std_app");
        modelBuilder.Entity<ClubMember>().ToTable("ClubMembers", "std_app");
        modelBuilder.Entity<University>().ToTable("Universities", "std_app");
        
        modelBuilder.Entity<User>().ToTable("AspNetUsers", "auth");
        modelBuilder.Entity<IdentityRole>().ToTable("AspNetRoles", "auth");
        modelBuilder.Entity<IdentityUserRole<string>>().ToTable("AspNetUserRoles", "auth");
        modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("AspNetUserClaims", "auth");
        modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("AspNetUserLogins", "auth");
        modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("AspNetRoleClaims", "auth");
        modelBuilder.Entity<IdentityUserToken<string>>().ToTable("AspNetUserTokens", "auth");
    }
    
    
}