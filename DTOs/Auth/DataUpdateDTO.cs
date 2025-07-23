namespace Backend.DTOs.Auth;

public class DataUpdateDTO
{
    
    public string? Username { get; set; } = string.Empty;
    public string? Email { get; set; } = string.Empty;
    public string? Name { get; set; } = string.Empty;
    public string? Surname { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; } = string.Empty;
}