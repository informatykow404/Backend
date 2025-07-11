using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Backend.EntityFramework.Models;

public class User : IdentityUser
{
    public string? Name { get; set; }
    public string? Surname { get; set; }
    //public University University { get; set; } 
    public virtual ICollection<ScienceClub> ScienceClubs { get; set; } 
}