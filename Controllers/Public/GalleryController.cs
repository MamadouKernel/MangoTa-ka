using Microsoft.AspNetCore.Mvc;
using MangoTaikaDistrict.Infrastructure.Repositories;

namespace MangoTaikaDistrict.Controllers.Public;

public class GalleryController : Controller
{
    private readonly IContentRepository _content;
    public GalleryController(IContentRepository content) => _content = content;

    public async Task<IActionResult> Index()
        => View(await _content.GetPublicAlbumsAsync());
}
