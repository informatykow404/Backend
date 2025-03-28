using System.ComponentModel.DataAnnotations;
using Backend.EntityFramework.Models.Enums;

namespace Backend.EntityFramework.Models;

public class ClubMember
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public required User User { get; set; }
    public required ScienceClub Club { get; set; }
    public ScienceClubRole Role { get; set; }
}

