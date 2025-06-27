using Backend.Data.Models;
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
        public async Task<IActionResult> GetById(int id, CancellationToken ct = default)
        {
            var club = await _scienceClubService.GetByIdAsync(id, ct);
            if (club is null) return NotFound();
            return Ok(club);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ScienceClub club, CancellationToken ct = default)
        {
            if (club is null) return BadRequest(Labels.ScienceClubController_InvalidInput);
            var createdClub = await _scienceClubService.CreateAsync(club, ct);
            return CreatedAtAction(nameof(GetById), new { id = createdClub.Id }, createdClub);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ScienceClub club, CancellationToken ct = default)
        {
            if (club is null) return BadRequest(Labels.ScienceClubController_InvalidInput);
            var updated = await _scienceClubService.UpdateAsync(id, club, ct);
            if (!updated) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
        {
            var deleted = await _scienceClubService.DeleteAsync(id, ct);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
