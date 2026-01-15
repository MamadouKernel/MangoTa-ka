using Microsoft.AspNetCore.Mvc;

namespace MangoTaikaDistrict.Controllers.Public;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
