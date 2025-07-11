using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Backend.EntityFramework.Contexts;
using Backend.EntityFramework.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace JWTAuthentication.NET6._0.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticateController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<User> _userManager;

    private readonly DataContext _context;
    public AuthenticateController(
        DataContext context,
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _context = context;
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            var authClaims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Name, user.UserName),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), //why?    
                new(JwtRegisteredClaimNames.Email, user.Email)
            };
            foreach (var VARIABLE in authClaims)
            {
                Console.WriteLine($"{VARIABLE.Type}  {VARIABLE.Value}");
            }


            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                Console.WriteLine($"Claim: {ClaimTypes.Role} {userRole}");
            }
            var token = GetToken(authClaims);
            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            });
        }

        return Unauthorized();
    }

    
    [HttpPost]
    [Route("refresh")]
    public async Task<IActionResult> Refresh()
    {
        string token = null;
        
        var refreshTokens = _context.RefreshTokens
            .Where(u => u.Token == token)
            .ToList();

        if (refreshTokens.Count == 0)
        {
            return Unauthorized();
        }

        if (refreshTokens.Count > 1)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new Response { Status = "Error", Message = "Multiple tokens found!" });
        }
        
        var user = await _userManager.FindByIdAsync(refreshTokens[0].UserId.ToString());

        var newRefreshToken = new RefreshTokens()
        {
            UserId = new Guid(user.Id),
            Token = GenerateRefreshToken(),
            Expires = DateTime.Now.AddDays(14)
        };
        
        await _context.RefreshTokens.AddAsync(newRefreshToken);
        await _context.SaveChangesAsync();

        {
            var userRoles = await _userManager.GetRolesAsync(user);
            var authClaims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Name, user.UserName),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), //why?
                new(JwtRegisteredClaimNames.Email, user.Email)
            };
            foreach (var VARIABLE in authClaims)
            {
                Console.WriteLine($"{VARIABLE.Type}  {VARIABLE.Value}");
            }


            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                Console.WriteLine($"Claim: {ClaimTypes.Role} {userRole}");
            }
            var securityToken = GetToken(authClaims);
            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                expiration = securityToken.ValidTo,
                refreshToken = newRefreshToken.Token,
                refreshTokenExpiration = newRefreshToken.Expires
            });
        }


        return Ok(new 
        {
            token = newRefreshToken.Token,
            expiration = newRefreshToken.Expires//??
        });
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        var userExists = await _userManager.FindByNameAsync(model.Username!);
        
        Console.WriteLine(userExists);
        if (userExists != null)
            return StatusCode(StatusCodes.Status500InternalServerError,
                new Response { Status = "Error", Message = "User already exists!" });

        User user = new()
        {
            Email = model.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = model.Username,
            Name = model.Username,
            //University = model.University!,
            ScienceClubs = new List<ScienceClub> { }
        };

        IdentityResult result;
        try
        {
            result = await _userManager.CreateAsync(user, model.Password!);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
        foreach (var variable in result.Errors) Console.WriteLine(variable.Description);
        if (!result.Succeeded)
            return StatusCode(StatusCodes.Status500InternalServerError,
                new Response
                    { Status = "Error", Message = "User creation failed! Please check user details and try again." });
        return Ok(new Response { Status = "Success", Message = "User created successfully!" });
    }

#if DEBUG
    [HttpPost]
    [Route("register-admin")]
    public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModel model)
    {
        var userExists = await _userManager.FindByNameAsync(model.Username!);
        if (userExists != null)
            return StatusCode(StatusCodes.Status500InternalServerError,
                new Response { Status = "Error", Message = "User already exists!" });
        User user = new()
        {
            Email = model.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = model.Username,
            //University = model.University!,
            ScienceClubs = new List<ScienceClub> { }
        };
        var result = await _userManager.CreateAsync(user, model.Password!);
        if (!result.Succeeded)
            return StatusCode(StatusCodes.Status500InternalServerError,
                new Response
                    { Status = "Error", Message = "User creation failed! Please check user details and try again." });
        if (!await _roleManager.RoleExistsAsync(UserRoles.SysAdmin))
            await _roleManager.CreateAsync(new IdentityRole(UserRoles.SysAdmin));
        if (!await _roleManager.RoleExistsAsync(UserRoles.User))
            await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));
        if (await _roleManager.RoleExistsAsync(UserRoles.SysAdmin))
            await _userManager.AddToRoleAsync(user, UserRoles.SysAdmin);
        if (await _roleManager.RoleExistsAsync(UserRoles.User))
            await _userManager.AddToRoleAsync(user, UserRoles.User);
        return Ok(new Response { Status = "Success", Message = "User created successfully!" });
    }

#endif
    
    private JwtSecurityToken GetToken(List<Claim> authClaims)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Authentication:JwtSecret"]!));
        var tokenValidity = int.Parse(_configuration["Authentication:TokenExpirationInMinutes"]!);
        var token = new JwtSecurityToken(
            _configuration["Authentication:ValidIssuer"],
            _configuration["Authentication:ValidAudience"],
            expires: DateTime.Now.AddMinutes(tokenValidity), //TODO: 15 minutes, refresh 14 days
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );
        return token;
    }
    
    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}

public class RegisterModel
{
    [Required(ErrorMessage = "User Name is required")]
    public string? Username { get; set; }

    [EmailAddress]
    [Required(ErrorMessage = "Email is required")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    public string? Password { get; set; }

    //TODO: reconsider using this
    //[Required(ErrorMessage = "University data is required")]
    //public University? University { get; set; }
    //
}

public class Response
{
    public string? Status { get; set; }
    public string? Message { get; set; }
}

public class UserRoles
{
    public const string SysAdmin = "SystemAdmin";
    public const string OrgAdmin = "OrganizationAdmin";
    public const string SysMod = "SystemModerator";
    public const string User = "User";
}

public class LoginModel
{
    [Required(ErrorMessage = "User Email is required")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    public string? Password { get; set; }
}