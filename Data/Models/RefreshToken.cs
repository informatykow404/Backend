namespace Backend.Data.Models;

public class RefreshToken
{
    public string Id { get; set; }
    public string UserId { get; set; }
    public string Token { get; set; }
    public DateTime Expires { get; set; }
    public bool Valid { get; set; }
    public User User { get; set; }
    
}