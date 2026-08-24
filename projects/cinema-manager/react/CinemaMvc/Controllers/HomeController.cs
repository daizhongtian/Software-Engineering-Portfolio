using Microsoft.AspNetCore.Mvc;

namespace CinemaMvc.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return Content("CinemaMvc Task 3 API is running. Use the React ClientApp for the frontend.");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return Content("An error occurred while processing your request.");
    }
}
