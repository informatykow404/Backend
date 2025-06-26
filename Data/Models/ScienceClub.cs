using System.ComponentModel.DataAnnotations;

namespace Backend.EntityFramework.Models;

public class ScienceClub
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public virtual ICollection<User> Users { get; set; }
}