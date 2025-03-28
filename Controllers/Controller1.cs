using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("EasterEgg")]
public class Controller1 : Controller
{
    
    [HttpGet]
    [Authorize]
    public IActionResult Index()
    {
        return Ok("Drogi Towarzyszu, gratuluję odnalezienia tego easter egga. W nagrodę otrzymujesz wirtualny uścisk dłoni od programisty, który to napisał. Miłego dnia!");
    }
}