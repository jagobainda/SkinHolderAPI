using Microsoft.AspNetCore.Mvc;

namespace SkinHolderAPI.Controllers;

public class HomeController : Controller
{
    [HttpGet("/")]
    public IActionResult Index() => View();
}