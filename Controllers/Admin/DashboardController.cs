using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MangoTaikaDistrict.Controllers.Admin;

[Authorize]
[Area("Admin")]
public class DashboardController : Controller
{
    public IActionResult Index() => View();
}
