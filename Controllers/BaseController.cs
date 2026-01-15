using Microsoft.AspNetCore.Mvc;

namespace MangoTaikaDistrict.Controllers
{
    public class BaseController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
