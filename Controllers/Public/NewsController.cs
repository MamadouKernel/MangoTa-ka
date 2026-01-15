using Microsoft.AspNetCore.Mvc;
using MangoTaikaDistrict.Infrastructure.Repositories;

namespace MangoTaikaDistrict.Controllers.Public;

public class NewsController : Controller
{
    private readonly IContentRepository _content;
    public NewsController(IContentRepository content) => _content = content;

    public async Task<IActionResult> Index()
        => View(await _content.GetPublishedNewsAsync());
}