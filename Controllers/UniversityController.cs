
using Backend.DTOs.University;
using Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/[controller]")]
public class UniversityController : ControllerBase
{
    IUniversityService _universityRepository;

    public UniversityController(IUniversityService universityService)
    {
        _universityRepository = universityService;
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPendingClubs([FromRoute] string id, CancellationToken ct = default)
    {
        var actionOutcome = await _universityRepository.GetPendingClubsAsync(id, ct);
        return Ok(actionOutcome);
    }
    
    [HttpPatch("acceptClub/{id}")]
    public async Task<IActionResult> AcceptClub([FromRoute] string id, AcceptDTO approval, CancellationToken ct = default)
    {
        var actionOutcome = await _universityRepository.AcceptClubAsync(id, approval, ct);
        if (actionOutcome.Item1)
            return Ok(actionOutcome.Item2);
        return BadRequest(actionOutcome.Item2);
    }

    [HttpGet("{id}/scienceClubs")]
    public async Task<IActionResult> GetScienceClubs([FromRoute] string id, CancellationToken ct = default)
    {
        var actionOutcome = await _universityRepository.GetActiveScienceClubsByUniversityAsync(id, ct);
        return Ok(actionOutcome);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateUniversity(CreateUniDTO uniData, CancellationToken ct = default)
    {
        var actionOutcome = await _universityRepository.CreateUniversityAsync(uniData, ct);
        if (actionOutcome.Item1)
            return Ok(actionOutcome.Item2);
        return BadRequest(actionOutcome.Item2);
    }

    /*TO DO*/
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> RemoveUniversity([FromRoute] string id, CancellationToken ct = default)
    {
        var actionOutcome = await _universityRepository.RemoveUniversityAsync(id, ct);
        if (actionOutcome.Item1)
            return Ok(actionOutcome.Item2);
        return BadRequest(actionOutcome.Item2);
    }

    [HttpPatch("{id}")]
    [Authorize(Roles = "University")]
    public async Task<IActionResult> UpdateUniversity([FromRoute] string id, UpdateUniDTO uniData, CancellationToken ct = default)
    {
        var actionOutcome = await _universityRepository.UpdateUniversityAsync(id, uniData, ct);
        if (actionOutcome.Item1)
            return Ok(actionOutcome.Item2);
        return BadRequest(actionOutcome.Item2);
    }
}