using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MangoTaikaDistrict.Domain.Entities;
using MangoTaikaDistrict.Domain.Enums;
using MangoTaikaDistrict.Infrastructure.Repositories;
using MangoTaikaDistrict.Infrastructure.Services;

namespace MangoTaikaDistrict.Controllers.Admin;

[Authorize(Roles = "ADMIN,GESTIONNAIRE")]
[Area("Admin")]
public class CotisationsController : Controller
{
    private readonly ICotisationRepository _cotisations;
    private readonly IScoutRepository _scouts;
    private readonly IGroupeRepository _groupes;
    private readonly IExcelImportService _excelImport;

    public CotisationsController(
        ICotisationRepository cotisations,
        IScoutRepository scouts,
        IGroupeRepository groupes,
        IExcelImportService excelImport)
    {
        _cotisations = cotisations;
        _scouts = scouts;
        _groupes = groupes;
        _excelImport = excelImport;
    }

    public async Task<IActionResult> Index(Guid? groupeId, string? periode, DateTime? start, DateTime? end)
    {
        ViewBag.Groupes = await _groupes.GetAllAsync();
        ViewBag.SelectedGroupeId = groupeId;
        ViewBag.Periode = periode ?? "";
        ViewBag.Start = start?.ToString("yyyy-MM-dd") ?? "";
        ViewBag.End = end?.ToString("yyyy-MM-dd") ?? "";

        var cotisations = await _cotisations.GetFilteredAsync(groupeId, periode, start, end);
        return View(cotisations);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.Scouts = await _scouts.GetAllAsync();
        ViewBag.Groupes = await _groupes.GetAllAsync();
        ViewBag.Statuts = Enum.GetValues<StatutCotisation>();
        return View(new Cotisation { Periode = DateTime.Now.Year.ToString() });
    }

    [HttpPost]
    public async Task<IActionResult> Create(Cotisation c)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Scouts = await _scouts.GetAllAsync();
            ViewBag.Groupes = await _groupes.GetAllAsync();
            ViewBag.Statuts = Enum.GetValues<StatutCotisation>();
            return View(c);
        }

        // Calculer le statut automatiquement si nécessaire
        if (c.MontantPaye >= c.MontantAttendu)
            c.Statut = StatutCotisation.PAYE;
        else if (c.MontantPaye > 0)
            c.Statut = StatutCotisation.PARTIEL;
        else
            c.Statut = StatutCotisation.IMPAYE;

        // Si payé, définir la date de paiement
        if (c.Statut == StatutCotisation.PAYE && !c.DatePaiement.HasValue)
            c.DatePaiement = DateTime.UtcNow;

        // Enregistrer l'utilisateur connecté
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (Guid.TryParse(userIdClaim, out var userId))
            c.EnregistreParId = userId;

        await _cotisations.AddAsync(c);
        await _cotisations.SaveAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var c = await _cotisations.GetByIdAsync(id);
        if (c is null) return NotFound();

        ViewBag.Scouts = await _scouts.GetAllAsync();
        ViewBag.Groupes = await _groupes.GetAllAsync();
        ViewBag.Statuts = Enum.GetValues<StatutCotisation>();
        return View(c);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Cotisation c)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Scouts = await _scouts.GetAllAsync();
            ViewBag.Groupes = await _groupes.GetAllAsync();
            ViewBag.Statuts = Enum.GetValues<StatutCotisation>();
            return View(c);
        }

        var db = await _cotisations.GetByIdAsync(c.Id);
        if (db is null) return NotFound();

        // Calculer le statut automatiquement
        if (c.MontantPaye >= c.MontantAttendu)
            c.Statut = StatutCotisation.PAYE;
        else if (c.MontantPaye > 0)
            c.Statut = StatutCotisation.PARTIEL;
        else
            c.Statut = StatutCotisation.IMPAYE;

        // Si payé, définir la date de paiement
        if (c.Statut == StatutCotisation.PAYE && !c.DatePaiement.HasValue)
            c.DatePaiement = DateTime.UtcNow;
        else if (c.Statut != StatutCotisation.PAYE)
            c.DatePaiement = null;

        db.ScoutId = c.ScoutId;
        db.GroupeId = c.GroupeId;
        db.Periode = c.Periode;
        db.MontantAttendu = c.MontantAttendu;
        db.MontantPaye = c.MontantPaye;
        db.Statut = c.Statut;
        db.DatePaiement = c.DatePaiement;
        db.Observations = c.Observations;
        db.UpdatedAt = DateTime.UtcNow;

        await _cotisations.SaveAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var c = await _cotisations.GetByIdAsync(id);
        if (c is null) return NotFound();
        return View(c);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(Guid id)
    {
        var c = await _cotisations.GetByIdAsync(id);
        if (c is null) return NotFound();
        return View(c);
    }

    [HttpPost]
    [ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var c = await _cotisations.GetByIdAsync(id);
        if (c is null) return NotFound();

        await _cotisations.DeleteAsync(c);
        await _cotisations.SaveAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Import()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Import(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            TempData["Error"] = "Veuillez sélectionner un fichier Excel.";
            return View();
        }

        if (!file.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase) &&
            !file.FileName.EndsWith(".xls", StringComparison.OrdinalIgnoreCase))
        {
            TempData["Error"] = "Le fichier doit être au format Excel (.xlsx ou .xls).";
            return View();
        }

        try
        {
            using var stream = file.OpenReadStream();
            var (success, errors, errorMessages) = await _excelImport.ImportCotisationsAsync(stream);

            if (errors > 0)
            {
                TempData["Warning"] = $"{success} cotisation(s) importée(s) avec succès, {errors} erreur(s).";
                if (errorMessages.Any())
                {
                    TempData["ErrorDetails"] = string.Join("\n", errorMessages.Take(10));
                }
            }
            else
            {
                TempData["Success"] = $"{success} cotisation(s) importée(s) avec succès.";
            }

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Erreur lors de l'import: {ex.Message}";
            return View();
        }
    }
}
