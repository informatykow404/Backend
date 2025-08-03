using Backend.Data.Models.Enums;

namespace Backend.DTOs.Auth;

public class GetInfoAboutUser
{
  public string? Id { get; set; }
  public string? PhoneNumber { get; set; }
  public string? Email { get; set; }
  public string? Name { get; set; }
  public string? Surname { get; set; }
  public string? UserName { get; set; }
  public string? Role { get; set; }
}