using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers.Ogranization;


[Route("api/organization/administration")]
[ApiController]
public class AdministrationController : ControllerBase
{
    
    [Authorize]
    [HttpPost]
    [Route("add")]
    public IActionResult AddClub()
    {
        return Ok();
    }
    
    
    [HttpPost]
    [Route("remove")]
    public IActionResult RemoveClub()
    {
        return Ok();
    }
    
    [HttpPost]
    [Route("edit")]
    public IActionResult EditClub()
    {
        return Ok();
    }
    
    [HttpGet]
    [Route("getlist")]
    public IActionResult GetClubList() //1 admin many clubs
    {
        return Ok();
    }
}