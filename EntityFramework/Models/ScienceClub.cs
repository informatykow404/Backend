using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Backend.EntityFramework.Models;

public class ScienceClub
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();
    public required string Name { get; set; }
    public required ClubStatus Status { get; set; }
    
    
    //TODO: description and details
}

public enum ClubStatus
{
    Active,
    Inactive,
    Pending
}