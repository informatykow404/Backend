using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Backend.Services.Interfaces;

public interface IJwtService
{
    JwtSecurityToken GenerateJwtToken(IEnumerable<Claim> claims);
}