
using Backend.DTOs.University;
using Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
/*[Authorize(Roles = "University, Admin")]*/
[Route("api/[controller]")]
public class UniversityController : ControllerBase
{
    IUniversityService _universityRepository;

    public UniversityController(IUniversityService universityService)
    {
        _universityRepository = universityService;
    }
    
    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetPendingClubs([FromRoute] string id, CancellationToken ct = default)
    {
        var actionOutcome = await _universityRepository.GetPendingClubsAsync(id, ct);
        return Ok(actionOutcome);
    }
    
    [HttpPatch("acceptClub/{id}")]
    /*[Authorize]*/
    public async Task<IActionResult> AcceptClub([FromRoute] string id, AcceptDTO approval, CancellationToken ct = default)
    {
        var actionOutcome = await _universityRepository.AcceptClubAsync(id, approval, ct);
        if (actionOutcome.Item1)
            return Ok(actionOutcome.Item2);
        return BadRequest(actionOutcome.Item2);
    }
}