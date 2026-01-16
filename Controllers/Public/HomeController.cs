using Microsoft.AspNetCore.Mvc;
using MangoTaikaDistrict.Infrastructure.Repositories;
using MangoTaikaDistrict.Domain.Entities;

namespace MangoTaikaDistrict.Controllers.Public;

public class HomeController : Controller
{
    private readonly IMotCommissaireRepository _motRepo;
    private readonly IContentRepository _contentRepo;

    public HomeController(IMotCommissaireRepository motRepo, IContentRepository contentRepo)
    {
        _motRepo = motRepo;
        _contentRepo = contentRepo;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.MotCommissaire = await _motRepo.GetActiveAsync();
        ViewBag.Banniere = await _contentRepo.GetPublishedNewsAsync();
        return View();
    }
}
