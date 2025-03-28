using System.ComponentModel.DataAnnotations;

namespace Backend.EntityFramework.Models;

public class University
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Name { get; set; }
    public required string Location { get; set; }
    public string? Description { get; set; }
    public virtual ICollection<ScienceClub>? ScienceClubs { get; set; }
    
}