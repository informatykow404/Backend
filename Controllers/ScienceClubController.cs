using Backend.Data.Models;
using Backend.Data.Models.Enums;
using Backend.DTOs.Auth;
using Backend.DTOs.ScienceClub;
using Backend.Services.Interfaces;
using Backend.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Authorize(Roles = "User,Admin")]
    [Route("api/[controller]")]
    public class ScienceClubController : ControllerBase
    {
        private readonly IScienceClubService _scienceClubService;
        public ScienceClubController(IScienceClubService scienceClubService)
        {
            _scienceClubService = scienceClubService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct = default)
        {
            var clubs = await _scienceClubService.GetAllAsync(ct);
            return Ok(clubs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id, CancellationToken ct = default)
        {
            var club = await _scienceClubService.GetByIdAsync(id, ct);
            if (club is null) return NotFound();
            return Ok(club);
        }

        /*[HttpPost]
        public async Task<IActionResult> Create([FromBody] ScienceClub club, CancellationToken ct = default)
        {
            if (club is null) return BadRequest(Labels.ScienceClubController_InvalidInput);
            var createdClub = await _scienceClubService.CreateAsync(club, ct);
            return CreatedAtAction(nameof(GetById), new { id = createdClub.Id }, createdClub);
        }*/

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] ScienceClub club, CancellationToken ct = default)
        {
            if (club is null) return BadRequest(Labels.ScienceClubController_InvalidInput);
            var updated = await _scienceClubService.UpdateAsync(id, club, ct);
            if (!updated) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id, CancellationToken ct = default)
        {
            var deleted = await _scienceClubService.DeleteAsync(id, ct);
            if (!deleted) return NotFound();
            return NoContent();
        }
        
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateScienceClub([FromBody] CreateDTO club, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(club.Name)) return BadRequest(Labels.ScienceClubController_InvalidInput);
            var token =  HttpContext.User.Claims.Select(c => new { c.Type, c.Value });
            var claim = token.FirstOrDefault(n => n.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
            var actionOutcome = await _scienceClubService.CreateClubAsync(club, claim.Value, ct);
            if (actionOutcome.Item1)
                return Ok(actionOutcome.Item2);
            return BadRequest(actionOutcome.Item2);
        }
        
        [HttpGet("{id}/join")]
        [Authorize]
        public async Task<IActionResult> JoinScienceClub([FromRoute] string id, CancellationToken ct = default)
        {
            var token =  HttpContext.User.Claims.Select(c => new { c.Type, c.Value });
            var claim = token.FirstOrDefault(n => n.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
            var actionOutcome = await _scienceClubService.JoinClubAsync(id, claim.Value, ct);
            if (actionOutcome.Item1)
                return Ok(actionOutcome.Item2);
            return BadRequest(actionOutcome.Item2);
        }
        
        [HttpGet("{id}/get-users")]
        [Authorize]
        public async Task<IActionResult> GetUsers([FromRoute] string id, CancellationToken ct = default)
        {
            var token =  HttpContext.User.Claims.Select(c => new { c.Type, c.Value });
            var claim = token.FirstOrDefault(n => n.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
            var actionOutcome = await _scienceClubService.GetUsersAsync(id, claim.Value, ct);
            if (actionOutcome.Item1)
                return Ok(actionOutcome.Item3);
            else if (actionOutcome is { Item1: false, Item2: "" })
                return Unauthorized();
            return BadRequest(actionOutcome.Item2);
        }
        
        [HttpPatch("{id}/modify-user-role")]
        [Authorize]
        public async Task<IActionResult> ModifyUserRole([FromRoute] string clubId, string userId, ScienceClubRole role, CancellationToken ct = default)
        {
            var token =  HttpContext.User.Claims.Select(c => new { c.Type, c.Value });
            var claim = token.FirstOrDefault(n => n.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
            var actionOutcome = await _scienceClubService.ModifyUserRoleAsync(clubId, userId, role, claim.Value, ct);
            if (actionOutcome.Item1)
                return Ok(actionOutcome.Item2);
            else if (actionOutcome.Item2 == "")
                return Unauthorized();
            return BadRequest(actionOutcome.Item2);
        }
    }
}
