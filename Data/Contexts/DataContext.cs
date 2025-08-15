using Backend.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Backend.Data.Models; 
using Backend.Data.Models.Enums;
using Microsoft.AspNetCore.Identity;

namespace Backend.EntityFramework.Contexts;

public class DataContext : IdentityDbContext<User>
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<ScienceClub> ScienceClubs { get; set; }
    
    public DbSet<ClubMember> ClubMembers { get; set; }
    
    public DbSet<University> Universities { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        //auth
        builder.Entity<User>().ToTable("AspNetUsers", "auth");
        builder.Entity<IdentityRole>().ToTable("AspNetRoles", "auth");
        builder.Entity<IdentityUserRole<string>>().ToTable("AspNetUserRoles", "auth");
        builder.Entity<IdentityUserClaim<string>>().ToTable("AspNetUserClaims", "auth");
        builder.Entity<IdentityUserLogin<string>>().ToTable("AspNetUserLogins", "auth");
        builder.Entity<IdentityUserToken<string>>().ToTable("AspNetUserTokens", "auth");
        builder.Entity<IdentityRoleClaim<string>>().ToTable("AspNetRoleClaims", "auth");
        builder.Entity<RefreshToken>().ToTable("RefreshTokens", "auth");

       
        
        //most_app
        builder.Entity<ScienceClub>().ToTable("ScienceClub", "most_app");
        builder.Entity<ClubMember>().ToTable("ClubMember", "most_app");
        builder.Entity<University>().ToTable("University", "most_app");
    }
    
    
}