using Backend.Data.Models;
using Backend.DTOs.Auth;
using Backend.Services.Implementations;
using Backend.Services.Interfaces;
using Backend.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Authorize(Roles = "User,Admin")]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        
        public UserController(IUserService userService)
        {
            _userService = userService;
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
        
        [HttpPost("DataModifier")]
        [Authorize]
        public async Task<IActionResult> DataModifier([FromBody] DataUpdateDTO request)
        {
            var token =  HttpContext.User.Claims.Select(c => new { c.Type, c.Value });
            var claim = token.FirstOrDefault(n => n.Type == "sub");
            var actionOutcome = await _userService.ReplaceData(request, claim.Value);
            if (actionOutcome.Item1)
                return Ok(new ResponseDTO
                {
                    Status = Labels.AuthenticateController_Success,
                    Message = actionOutcome.Item2
                });
            return BadRequest(actionOutcome.Item2);
        }
    }
}
