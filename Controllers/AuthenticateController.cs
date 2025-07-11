using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Backend.Data.Models;
using Backend.DTOs.Auth;
using Backend.EntityFramework.Contexts;
using Backend.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenticateController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<User> _userManager;
    private readonly DataContext _ctx;

    public AuthenticateController(
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration,
        DataContext ctx)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _ctx = ctx;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user != null && await _userManager.CheckPasswordAsync(user, request.Password))
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.UserName!),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email!)
            };
            claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

            var token = GenerateJwtToken(claims);
            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            });
        }

        return Unauthorized(new ResponseDTO { 
            Status = Labels.AuthenticateController_Unauthorized, 
            Message = Labels.AuthenticateController_InvalidCredentials });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDTO request)
    {
        if (await _userManager.FindByNameAsync(request.Username) is not null)
            return BadRequest(new ResponseDTO { 
                Status = Labels.AuthenticateController_Error, 
                Message = Labels.AuthenticateController_UserAlreadyExists
            });

        var user = new User
        {
            UserName = request.Username,
            Email = request.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            Name = request.Username
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            return StatusCode(500, new ResponseDTO { 
                Status = Labels.AuthenticateController_Error, 
                Message = errors });
        }

        await EnsureRoleExists(Labels.AuthenticateController_UserRole);
        await _userManager.AddToRoleAsync(user, Labels.AuthenticateController_UserRole);

        return Ok(new ResponseDTO { 
            Status = Labels.AuthenticateController_Success, 
            Message = Labels.AuthenticateController_UserRegistered
        });
    }
    
    [HttpPost("Logout")]
    [Authorize]
    public async Task<IActionResult> Logout([FromBody] LogoutDTO request)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            return BadRequest("Brak tokenu w żądaniu.");
        
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized();

        var ok = await RevokeRefreshTokenAsync(request.RefreshToken, userId);
        if (!ok)
            return NotFound("Token nie istnieje, jest już unieważniony lub wygasł.");

        return NoContent();
    }

    [HttpPost("register-admin")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RegisterAdmin([FromBody] RegisterDTO request)
    {
        if (await _userManager.FindByNameAsync(request.Username) is not null)
            return BadRequest(new ResponseDTO { 
                Status = Labels.AuthenticateController_Error,
                Message = Labels.AuthenticateController_UserAlreadyExists
            });

        var user = new User
        {
            UserName = request.Username,
            Email = request.Email,
            SecurityStamp = Guid.NewGuid().ToString()
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            return StatusCode(500, new ResponseDTO { 
                Status = Labels.AuthenticateController_Error,
                Message = Labels.AuthenticateController_UserCreationFailed
            });

        foreach (var role in new[] { Labels.AuthenticateController_UserRole, Labels.AuthenticateController_AdminRole })
        {
            await EnsureRoleExists(role);
            await _userManager.AddToRoleAsync(user, role);
        }

        return Ok(new ResponseDTO { 
            Status = Labels.AuthenticateController_Success,
            Message = Labels.AuthenticateController_AdminRegistered
        });
    }

    private JwtSecurityToken GenerateJwtToken(IEnumerable<Claim> claims)
    {
        // PRD
        // var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET");

        // DEV (remove on PRD)
        var jwtSecret = _configuration["JWT_SECRET"];

        var signingKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSecret));

        return new JwtSecurityToken(
            issuer: _configuration["Authentication:ValidIssuer"],
            audience: _configuration["Authentication:ValidAudience"],
            expires: DateTime.UtcNow.AddHours(3),
            claims: claims,
            signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
        );
    }

    private async Task EnsureRoleExists(string role)
    {
        if (!await _roleManager.RoleExistsAsync(role))
            await _roleManager.CreateAsync(new IdentityRole(role));
    }
    
    public async Task<bool> RevokeRefreshTokenAsync(string token, string userId)
    {
        var rt = await _ctx.RefreshTokens
            .SingleOrDefaultAsync(x => x.Token == token && x.UserId == userId);
        if (rt == null || rt.IsRevoked || rt.ExpiresAt < DateTime.UtcNow)
            return false;

        rt.IsRevoked = true;
        await _ctx.SaveChangesAsync();
        return true;
    }
}