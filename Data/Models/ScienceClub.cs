using System.ComponentModel.DataAnnotations;
using Backend.Data.Models.Enums;
namespace Backend.Data.Models;

public class ScienceClub
{
    [Key]
    public required string Id { get; set; }
    public required string? Name { get; set; }
    public virtual ICollection<User>? Users { get; set; }

    public required ClubStatus status;



}



