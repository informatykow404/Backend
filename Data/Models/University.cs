using System.ComponentModel.DataAnnotations;

namespace Backend.Data.Models;

public class University
{
    [Key]
    
    public required string Id { get; set; }
    
    public required string Name { get; set; }
    
    public required string Location { get; set; }
    
    public required string? Description { get; set; }
    
    public required ICollection<ScienceClub> Clubs { get; set; } = new List<ScienceClub>();
    
}