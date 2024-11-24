using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("we")]
public class Controller1 : Controller
{
    // GET
    [HttpGet]
    [Authorize]
    public IActionResult Index()
    {
        return Ok("asddddddddddddddddd");
    }
}