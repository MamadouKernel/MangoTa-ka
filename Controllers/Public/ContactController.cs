using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MangoTaikaDistrict.Application.Interfaces;
using MangoTaikaDistrict.Models.Public;
using MangoTaikaDistrict.Domain.Entities;
using MangoTaikaDistrict.Infrastructure.Data;

namespace MangoTaikaDistrict.Controllers.Public;

public class ContactController : Controller
{
    private readonly IEmailService _email;
    private readonly AppDbContext _db;

    public ContactController(IEmailService email, AppDbContext db)
    {
        _email = email;
        _db = db;
    }

    [HttpGet]
    public IActionResult Index() => View(new ContactVm());

    [HttpPost]
    public async Task<IActionResult> Index(ContactVm vm)
    {
        if (!ModelState.IsValid) return View(vm);

        if (!vm.Consentement)
        {
            ModelState.AddModelError(nameof(vm.Consentement), "Vous devez accepter le consentement pour continuer.");
            return View(vm);
        }

        // Enregistrer le consentement (on utilise un utilisateur système temporaire)
        // Note: Dans une vraie application, on enregistrerait aussi l'utilisateur s'il n'existe pas
        // Pour l'instant, on utilise le premier admin comme référence système
        var systemUser = await _db.Utilisateurs.FirstOrDefaultAsync(u => u.Telephone == "0100000000");
        if (systemUser != null)
        {
            var consentement = new Consentement
            {
                UtilisateurId = systemUser.Id, // Utilisateur système pour les consentements anonymes
                Version = "v1",
                AcceptedAt = DateTime.UtcNow,
                Ip = HttpContext.Connection.RemoteIpAddress?.ToString(),
                UserAgent = HttpContext.Request.Headers["User-Agent"].ToString()
            };
            await _db.Consentements.AddAsync(consentement);
        }

        // Envoyer l'email
        var emailSent = await _email.SendContactEmailAsync(
            vm.Nom,
            vm.Email,
            vm.Telephone ?? "",
            vm.Message,
            "Contact"
        );

        await _db.SaveChangesAsync();

        if (emailSent)
        {
            TempData["Success"] = "Votre message a été envoyé avec succès. Nous vous répondrons dans les plus brefs délais.";
        }
        else
        {
            TempData["Warning"] = "Votre message a été enregistré mais l'envoi de l'email a échoué. Veuillez réessayer ou nous contacter directement.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Suggestions() => View("Index", new ContactVm());

    [HttpPost]
    [ActionName("Suggestions")]
    public async Task<IActionResult> SuggestionsPost(ContactVm vm)
    {
        if (!ModelState.IsValid) return View("Index", vm);

        if (!vm.Consentement)
        {
            ModelState.AddModelError(nameof(vm.Consentement), "Vous devez accepter le consentement pour continuer.");
            return View("Index", vm);
        }

        // Enregistrer le consentement
        var consentement = new Consentement
        {
            UtilisateurId = Guid.Empty,
            Version = "v1",
            AcceptedAt = DateTime.UtcNow,
            Ip = HttpContext.Connection.RemoteIpAddress?.ToString(),
            UserAgent = HttpContext.Request.Headers["User-Agent"].ToString()
        };
        await _db.Consentements.AddAsync(consentement);

        // Envoyer l'email avec type "Suggestion"
        var emailSent = await _email.SendContactEmailAsync(
            vm.Nom,
            vm.Email,
            vm.Telephone ?? "",
            vm.Message,
            "Suggestion"
        );

        await _db.SaveChangesAsync();

        if (emailSent)
        {
            TempData["Success"] = "Merci pour votre suggestion ! Nous en prendrons connaissance et vous recontacterons si nécessaire.";
        }
        else
        {
            TempData["Warning"] = "Votre suggestion a été enregistrée mais l'envoi de l'email a échoué. Veuillez réessayer.";
        }

        return RedirectToAction(nameof(Suggestions));
    }
}
