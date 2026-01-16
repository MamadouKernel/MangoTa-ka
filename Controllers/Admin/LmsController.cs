using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MangoTaikaDistrict.Infrastructure.Repositories;
using System.Security.Claims;

namespace MangoTaikaDistrict.Controllers.Admin;

[Authorize(Roles = "ADMIN,GESTIONNAIRE")]
[Area("Admin")]
public class LmsController : Controller
{
    private readonly IFormationRepository _formations;
    private readonly IInscriptionFormationRepository _inscriptions;

    public LmsController(
        IFormationRepository formations,
        IInscriptionFormationRepository inscriptions)
    {
        _formations = formations;
        _inscriptions = inscriptions;
    }

    public async Task<IActionResult> Index()
    {
        var formations = await _formations.GetAllAsync(true);
        return View(formations);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new Domain.Entities.Formation());
    }

    [HttpPost]
    public async Task<IActionResult> Create(Domain.Entities.Formation formation)
    {
        if (!ModelState.IsValid) return View(formation);

        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        formation.CreatedById = userId;
        formation.CreatedAt = DateTime.UtcNow;
        formation.UpdatedAt = DateTime.UtcNow;

        await _formations.AddAsync(formation);
        await _formations.SaveAsync();

        TempData["Success"] = "Formation créée avec succès.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var formation = await _formations.GetByIdWithModulesAsync(id);
        if (formation == null) return NotFound();
        return View(formation);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Domain.Entities.Formation formation)
    {
        if (!ModelState.IsValid) return View(formation);

        formation.UpdatedAt = DateTime.UtcNow;
        await _formations.UpdateAsync(formation);
        await _formations.SaveAsync();

        TempData["Success"] = "Formation mise à jour avec succès.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Inscriptions(Guid formationId)
    {
        var formation = await _formations.GetByIdAsync(formationId);
        if (formation == null) return NotFound();

        var inscriptions = await _inscriptions.GetByFormationIdAsync(formationId);
        ViewBag.Formation = formation;
        return View(inscriptions);
    }

    [HttpPost]
    public async Task<IActionResult> GenererCertificat(Guid inscriptionId)
    {
        var inscription = await _inscriptions.GetByIdWithDetailsAsync(inscriptionId);
        if (inscription == null || inscription.Statut != Domain.Enums.StatutInscription.COMPLETE)
        {
            TempData["Error"] = "La formation doit être complétée pour générer un certificat.";
            return RedirectToAction(nameof(Inscriptions), new { formationId = inscription?.FormationId });
        }

        // Vérifier si certificat existe déjà
        if (inscription.Certificats.Any(c => c.EstValide))
        {
            TempData["Error"] = "Un certificat valide existe déjà pour cette formation.";
            return RedirectToAction(nameof(Inscriptions), new { formationId = inscription.FormationId });
        }

        var certificat = new Domain.Entities.Certificat
        {
            InscriptionFormationId = inscriptionId,
            NumeroCertificat = $"CERT-{DateTime.UtcNow:yyyy}-{Guid.NewGuid().ToString("N").Substring(0, 5).ToUpper()}",
            DateEmission = DateTime.UtcNow,
            EmisParId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!),
            EstValide = true
        };

        // TODO: Générer le PDF du certificat
        // certificat.UrlCertificat = await _pdfService.GenerateCertificatAsync(certificat);

        inscription.Certificats.Add(certificat);
        await _inscriptions.UpdateAsync(inscription);
        await _inscriptions.SaveAsync();

        TempData["Success"] = "Certificat généré avec succès.";
        return RedirectToAction(nameof(Inscriptions), new { formationId = inscription.FormationId });
    }
}
