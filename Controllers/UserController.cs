using System.Text.RegularExpressions;
using Backend.Data.Models;
using Backend.Services.Interfaces;
using Backend.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Authorize(Roles = "User,Admin")]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;
        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct = default)
        {
            var users = await _userService.GetAllAsync(ct);
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByGuid(string guid, CancellationToken ct = default)
        {
            var user = await _userService.GetByGuidAsync(guid, ct);
            if (user is null) return NotFound();
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] User user, CancellationToken ct = default)
        {
            if (user is null) return BadRequest(Labels.UserController_InvalidInput);
            var createdUser = await _userService.CreateAsync(user, ct);
            return CreatedAtAction(nameof(GetByGuid), new { id = createdUser.Id }, createdUser);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string guid, [FromBody] User user, CancellationToken ct = default)
        {
            if (user is null) return BadRequest(Labels.UserController_InvalidInput);
            var updated = await _userService.UpdateAsync(guid, user, ct);
            if (!updated) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string guid, CancellationToken ct = default)
        {
            var deleted = await _userService.DeleteAsync(guid, ct);
            if (!deleted) return NotFound();
            return NoContent();
        }
        
        
        [HttpGet("GetUserByNickname")]
        public async Task<IActionResult> GetUserByUsernameQuery([FromQuery] string username, CancellationToken ct = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username))
                {
                    return BadRequest("Username parameter is required");
                }

                var userInfo = await _userService.GetDataAboutUser(username, ct);

                if (userInfo == null)
                {
                    return NotFound(new { message = $"User with this username  not found" });
                }

                return Ok(userInfo);
            }
            catch (OperationCanceledException)
            {
                return BadRequest("Operation was cancelled");
            }
            catch (Exception ex)
            {
                var safeUsername = Regex.Replace(username ?? "unknown", @"[^\w\.\-@]", "_");
                safeUsername = safeUsername.Length > 30 ? safeUsername.Substring(0, 30) : safeUsername;
                _logger.LogError(ex, "Error getting user data for username");
                return StatusCode(500, "Internal server error");
            }
        
        
    }
    
   
    }
    
    
}
