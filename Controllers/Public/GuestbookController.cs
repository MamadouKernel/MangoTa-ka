using Microsoft.AspNetCore.Mvc;
using MangoTaikaDistrict.Domain.Entities;
using MangoTaikaDistrict.Infrastructure.Repositories;

namespace MangoTaikaDistrict.Controllers.Public;

public class GuestbookController : Controller
{
    private readonly IContentRepository _content;
    public GuestbookController(IContentRepository content) => _content = content;

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        ViewBag.Messages = await _content.GetGuestbookPublicAsync();
        return View(new LivreOrMessage());
    }

    [HttpPost]
    public async Task<IActionResult> Index(LivreOrMessage vm)
    {
        if (string.IsNullOrWhiteSpace(vm.Auteur) || string.IsNullOrWhiteSpace(vm.Message))
        {
            ModelState.AddModelError("", "Auteur et message sont obligatoires.");
            ViewBag.Messages = await _content.GetGuestbookPublicAsync();
            return View(vm);
        }

        await _content.AddGuestbookAsync(new LivreOrMessage { Auteur = vm.Auteur, Message = vm.Message });
        await _content.SaveAsync();

        TempData["ok"] = "Message envoyé. Il sera visible après validation.";
        return RedirectToAction(nameof(Index));
    }
}
