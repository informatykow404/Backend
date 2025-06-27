using System.ComponentModel.DataAnnotations;

namespace Backend.Data.Models;

public class ScienceClub
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public virtual ICollection<User> Users { get; set; }
}