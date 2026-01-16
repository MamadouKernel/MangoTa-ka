using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MangoTaikaDistrict.Domain.Entities;
using MangoTaikaDistrict.Infrastructure.Repositories;
using MangoTaikaDistrict.Infrastructure.Services;

namespace MangoTaikaDistrict.Controllers.Admin;

[Authorize(Roles = "ADMIN,GESTIONNAIRE")]
[Area("Admin")]
public class ScoutsController : Controller
{
    private readonly IScoutRepository _scouts;
    private readonly IGroupeRepository _groupes;
    private readonly IExcelImportService _excelImport;

    public ScoutsController(IScoutRepository scouts, IGroupeRepository groupes, IExcelImportService excelImport)
    {
        _scouts = scouts;
        _groupes = groupes;
        _excelImport = excelImport;
    }

    public async Task<IActionResult> Index()
        => View(await _scouts.GetAllAsync());

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.Groupes = await _groupes.GetAllAsync();
        return View(new Scout());
    }

    [HttpPost]
    public async Task<IActionResult> Create(Scout s)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Groupes = await _groupes.GetAllAsync();
            return View(s);
        }

        await _scouts.AddAsync(s);
        await _scouts.SaveAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var s = await _scouts.GetByIdAsync(id);
        if (s is null) return NotFound();
        ViewBag.Groupes = await _groupes.GetAllAsync();
        return View(s);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Scout s)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Groupes = await _groupes.GetAllAsync();
            return View(s);
        }

        var db = await _scouts.GetByIdAsync(s.Id);
        if (db is null) return NotFound();

        db.Matricule = s.Matricule;
        db.Nom = s.Nom;
        db.Prenoms = s.Prenoms;
        db.Sexe = s.Sexe;
        db.GroupeId = s.GroupeId;
        db.Statut = s.Statut;
        db.UpdatedAt = DateTime.UtcNow;

        await _scouts.SaveAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var s = await _scouts.GetByIdAsync(id);
        if (s is null) return NotFound();
        return View(s);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(Guid id)
    {
        var s = await _scouts.GetByIdAsync(id);
        if (s is null) return NotFound();
        return View(s);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var s = await _scouts.GetByIdAsync(id);
        if (s is null) return NotFound();

        await _scouts.DeleteAsync(s);
        await _scouts.SaveAsync();
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
            var (success, errors, errorMessages) = await _excelImport.ImportScoutsAsync(stream);

            if (errors > 0)
            {
                TempData["Warning"] = $"{success} scout(s) importé(s) avec succès, {errors} erreur(s).";
                if (errorMessages.Any())
                {
                    TempData["ErrorDetails"] = string.Join("\n", errorMessages.Take(10)); // Limiter à 10 erreurs
                }
            }
            else
            {
                TempData["Success"] = $"{success} scout(s) importé(s) avec succès.";
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
