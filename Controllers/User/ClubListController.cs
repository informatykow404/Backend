using Backend.EntityFramework.Contexts;
using Backend.EntityFramework.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers.User;

[Route("api/clubs")]
[ApiController]
public class ClubListController : ControllerBase
{
    private readonly DataContext _context;
    
    public ClubListController(DataContext context)
    {
        _context = context;
    }
    
    [Authorize]
    [HttpPost]
    [Route("getlist")]
    public IActionResult GetClubList()
    {
        //TODO: Implement categories

        //TODO: only basic info
        
        _context.ScienceClubs
            .Where(u => u.Status == ClubStatus.Active)
            .ToList();
        
        
        return Ok();
    }
    
    
    [Authorize]
    [HttpPost]
    [Route("getinfo")]
    public IActionResult GetClubInfo()
    {
        return Ok();
    }
    
}