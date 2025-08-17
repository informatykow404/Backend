namespace Backend.Data.Models;

public class RefreshToken
{
    public required string Id { get; set; }
    public required User User { get; set; }
    public required string Token { get; set; }
    public required DateTime Expires { get; set; }
    public required bool Valid { get; set; }  
}