using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MangoTaikaDistrict.Domain.Entities;
using MangoTaikaDistrict.Domain.Enums;
using MangoTaikaDistrict.Infrastructure.Repositories;
using MangoTaikaDistrict.Application.Services;
using MangoTaikaDistrict.Infrastructure.Services;

namespace MangoTaikaDistrict.Controllers.Admin;

[Authorize(Roles = "ADMIN,GESTIONNAIRE")]
[Area("Admin")]
public class NominationsController : Controller
{
    private readonly INominationRepository _nominations;
    private readonly IScoutRepository _scouts;
    private readonly IGroupeRepository _groupes;
    private readonly Infrastructure.Data.AppDbContext _db;
    private readonly WorkflowService _workflowService;
    private readonly IWorkflowRepository _workflowRepo;
    private readonly IExcelImportService _excelImport;

    public NominationsController(
        INominationRepository nominations,
        IScoutRepository scouts,
        IGroupeRepository groupes,
        Infrastructure.Data.AppDbContext db,
        WorkflowService workflowService,
        IWorkflowRepository workflowRepo,
        IExcelImportService excelImport)
    {
        _nominations = nominations;
        _scouts = scouts;
        _groupes = groupes;
        _db = db;
        _workflowService = workflowService;
        _workflowRepo = workflowRepo;
        _excelImport = excelImport;
    }

    public async Task<IActionResult> Index(Guid? groupeId, Guid? scoutId, DateTime? start, DateTime? end)
    {
        ViewBag.Groupes = await _groupes.GetAllAsync();
        ViewBag.SelectedGroupeId = groupeId;
        ViewBag.SelectedScoutId = scoutId;
        ViewBag.Start = start?.ToString("yyyy-MM-dd") ?? "";
        ViewBag.End = end?.ToString("yyyy-MM-dd") ?? "";

        var nominations = await _nominations.GetFilteredAsync(groupeId, scoutId, start, end);
        return View(nominations);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.Scouts = await _scouts.GetAllAsync();
        ViewBag.Groupes = await _groupes.GetAllAsync();
        ViewBag.Statuts = Enum.GetValues<StatutNomination>();
        ViewBag.Utilisateurs = await _db.Utilisateurs.ToListAsync();
        return View(new Nomination 
        { 
            DateDebut = DateOnly.FromDateTime(DateTime.Now),
            Statut = StatutNomination.BROUILLON
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create(Nomination n)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Scouts = await _scouts.GetAllAsync();
            ViewBag.Groupes = await _groupes.GetAllAsync();
            ViewBag.Statuts = Enum.GetValues<StatutNomination>();
            ViewBag.Utilisateurs = await _db.Utilisateurs.ToListAsync();
            return View(n);
        }

        // Validation des dates
        if (n.DateFin.HasValue && n.DateFin.Value < n.DateDebut)
        {
            ModelState.AddModelError("DateFin", "La date de fin doit être postérieure à la date de début.");
            ViewBag.Scouts = await _scouts.GetAllAsync();
            ViewBag.Groupes = await _groupes.GetAllAsync();
            ViewBag.Statuts = Enum.GetValues<StatutNomination>();
            ViewBag.Utilisateurs = await _db.Utilisateurs.ToListAsync();
            return View(n);
        }

        // Enregistrer l'utilisateur connecté
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (Guid.TryParse(userIdClaim, out var userId))
            n.CreeParId = userId;

        await _nominations.AddAsync(n);
        await _nominations.SaveAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var n = await _nominations.GetByIdAsync(id);
        if (n is null) return NotFound();

        ViewBag.Scouts = await _scouts.GetAllAsync();
        ViewBag.Groupes = await _groupes.GetAllAsync();
        ViewBag.Statuts = Enum.GetValues<StatutNomination>();
        ViewBag.Utilisateurs = await _db.Utilisateurs.ToListAsync();
        return View(n);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Nomination n)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Scouts = await _scouts.GetAllAsync();
            ViewBag.Groupes = await _groupes.GetAllAsync();
            ViewBag.Statuts = Enum.GetValues<StatutNomination>();
            ViewBag.Utilisateurs = await _db.Utilisateurs.ToListAsync();
            return View(n);
        }

        var db = await _nominations.GetByIdAsync(n.Id);
        if (db is null) return NotFound();

        // Validation des dates
        if (n.DateFin.HasValue && n.DateFin.Value < n.DateDebut)
        {
            ModelState.AddModelError("DateFin", "La date de fin doit être postérieure à la date de début.");
            ViewBag.Scouts = await _scouts.GetAllAsync();
            ViewBag.Groupes = await _groupes.GetAllAsync();
            ViewBag.Statuts = Enum.GetValues<StatutNomination>();
            ViewBag.Utilisateurs = await _db.Utilisateurs.ToListAsync();
            return View(n);
        }

        db.ScoutId = n.ScoutId;
        db.GroupeId = n.GroupeId;
        db.Poste = n.Poste;
        db.Fonction = n.Fonction;
        db.DateDebut = n.DateDebut;
        db.DateFin = n.DateFin;
        db.Statut = n.Statut;
        db.AutoriteValidationId = n.AutoriteValidationId;
        db.Observations = n.Observations;
        db.UpdatedAt = DateTime.UtcNow;

        await _nominations.SaveAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var n = await _nominations.GetByIdAsync(id);
        if (n is null) return NotFound();
        return View(n);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(Guid id)
    {
        var n = await _nominations.GetByIdAsync(id);
        if (n is null) return NotFound();
        return View(n);
    }

    [HttpPost]
    [ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var n = await _nominations.GetByIdAsync(id);
        if (n is null) return NotFound();

        await _nominations.DeleteAsync(n);
        await _nominations.SaveAsync();
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "ADMIN,GESTIONNAIRE")]
    [HttpPost]
    public async Task<IActionResult> Soumettre(Guid id)
    {
        var nomination = await _nominations.GetByIdAsync(id);
        if (nomination is null) return NotFound();

        if (nomination.Statut != StatutNomination.BROUILLON)
        {
            TempData["Error"] = "Seules les nominations en brouillon peuvent être soumises.";
            return RedirectToAction(nameof(Details), new { id });
        }

        // TODO: Implémenter le workflow de validation pour les nominations
        // Pour l'instant, on passe directement à EN_ATTENTE
        nomination.Statut = StatutNomination.EN_ATTENTE;
        nomination.CurrentStep = 1;
        nomination.UpdatedAt = DateTime.UtcNow;

        await _nominations.SaveAsync();
        TempData["Success"] = "Nomination soumise avec succès.";
        return RedirectToAction(nameof(Details), new { id });
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
            var (success, errors, errorMessages) = await _excelImport.ImportNominationsAsync(stream);

            if (errors > 0)
            {
                TempData["Warning"] = $"{success} nomination(s) importée(s) avec succès, {errors} erreur(s).";
                if (errorMessages.Any())
                {
                    TempData["ErrorDetails"] = string.Join("\n", errorMessages.Take(10));
                }
            }
            else
            {
                TempData["Success"] = $"{success} nomination(s) importée(s) avec succès.";
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
