using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Backend.Data.Models.Enums;

namespace Backend.Data.Models;

public class User : IdentityUser
{
    public string? Name { get; set; }
    public string? Surname { get; set; }
    
    
}