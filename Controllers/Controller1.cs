using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("we")]
public class Controller1 : Controller
{
    // GET
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
}