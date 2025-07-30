using System.ComponentModel.DataAnnotations;
 using Backend.Data.Models.Enums;
namespace Backend.Data.Models;

public class ClubMember
{
    [Key] 
    
    public required string Id { get; set; }
    
    public required User User { get; set; }
    
    public required ScienceClub Club { get; set; }
    
   public ScienceClubRole Role { get; set; }
}




