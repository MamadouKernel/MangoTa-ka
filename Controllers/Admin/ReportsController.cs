using ClosedXML.Excel;
using MangoTaikaDistrict.Domain.Entities;
using MangoTaikaDistrict.Infrastructure.Data;
using MangoTaikaDistrict.Infrastructure.Pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;

namespace MangoTaikaDistrict.Controllers.Admin;

[Authorize(Roles = "ADMIN,GESTIONNAIRE,SUPERVISEUR")]
[Area("Admin")]
public class ReportsController : Controller
{
    private readonly AppDbContext _db;
    public ReportsController(AppDbContext db) => _db = db;

   
    public async Task<IActionResult> Index(Guid? groupeId, DateTime? start, DateTime? end)
    {
        ViewBag.Groupes = await _db.Groupes.OrderBy(g => g.Nom).ToListAsync();

        ViewBag.SelectedGroupeId = groupeId;
        ViewBag.Start = start?.ToString("yyyy-MM-dd") ?? "";
        ViewBag.End = end?.ToString("yyyy-MM-dd") ?? "";

        // Stats globales (ou filtrées si tu veux)
        ViewBag.NbScouts = await _db.Scouts.CountAsync();
        ViewBag.NbGroupes = await _db.Groupes.CountAsync();
        ViewBag.NbTickets = await _db.Tickets.CountAsync();
        ViewBag.NbActivites = await _db.Activites.CountAsync();

        return View();
    }


    public async Task<IActionResult> ExportScoutsCsv()
    {
        var rows = await _db.Scouts.Include(s => s.Groupe).OrderBy(s => s.Nom).ToListAsync();
        var csv = "Matricule;Nom;Prenoms;Groupe;Statut\n" +
                  string.Join("\n", rows.Select(s =>
                      $"{s.Matricule};{s.Nom};{s.Prenoms};{s.Groupe?.Nom};{s.Statut}"
                  ));

        return File(System.Text.Encoding.UTF8.GetBytes(csv), "text/csv", "scouts.csv");
    }


    public async Task<IActionResult> ExportScoutsPdf()
    {
        var scouts = await _db.Scouts.Include(s => s.Groupe)
            .OrderBy(s => s.Nom).ToListAsync();

        var doc = new ScoutsPdfDocument(scouts);
        var bytes = doc.GeneratePdf();

        return File(bytes, "application/pdf", "scouts.pdf");
    }

    public async Task<IActionResult> ExportScoutsPdfOfficial(Guid? groupeId = null, DateTime? start = null, DateTime? end = null)
    {
        var query = _db.Scouts.Include(s => s.Groupe).AsQueryable();
        
        if (groupeId.HasValue)
            query = query.Where(s => s.GroupeId == groupeId.Value);
        
        if (start.HasValue)
            query = query.Where(s => s.CreatedAt >= start.Value);
        
        if (end.HasValue)
            query = query.Where(s => s.CreatedAt <= end.Value);
        
        var scouts = await query.OrderBy(s => s.Nom).ToListAsync();

        var data = new ScoutsReportData
        {
            GeneratedAtUtc = DateTime.UtcNow,
            TotalScouts = scouts.Count,
            TotalGroupes = await _db.Groupes.CountAsync(),
            ByGroupe = scouts
                .GroupBy(s => s.Groupe?.Nom ?? "N/A")
                .OrderByDescending(g => g.Count())
                .ToDictionary(g => g.Key, g => g.Count()),
            ByStatut = scouts
                .GroupBy(s => string.IsNullOrWhiteSpace(s.Statut) ? "N/A" : s.Statut)
                .OrderByDescending(g => g.Count())
                .ToDictionary(g => g.Key, g => g.Count()),
            BySexe = scouts
                .GroupBy(s => string.IsNullOrWhiteSpace(s.Sexe) ? "N/A" : s.Sexe)
                .OrderByDescending(g => g.Count())
                .ToDictionary(g => g.Key, g => g.Count())
        };

        // Logo depuis wwwroot/assets/logo.png
        byte[]? logoBytes = null;
        var env = HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>();
        var logoPath = Path.Combine(env.WebRootPath, "assets", "logo.png");
        if (System.IO.File.Exists(logoPath))
            logoBytes = await System.IO.File.ReadAllBytesAsync(logoPath);

        var doc = new ScoutsOfficialPdfDocument(data, scouts, logoBytes);
        var bytes = doc.GeneratePdf(); // QuestPDF output generation :contentReference[oaicite:4]{index=4}

        return File(bytes, "application/pdf", "rapport_scouts_officiel.pdf");
    }

    private IQueryable<Scout> ApplyScoutsFilters(IQueryable<Scout> q, Guid? groupeId, DateTime? start, DateTime? end)
    {
        if (groupeId.HasValue)
            q = q.Where(s => s.GroupeId == groupeId.Value);

        if (start.HasValue)
            q = q.Where(s => s.CreatedAt >= start.Value.Date);

        if (end.HasValue)
        {
            // inclusif sur la journée de fin
            var endInclusive = end.Value.Date.AddDays(1);
            q = q.Where(s => s.CreatedAt < endInclusive);
        }

        return q;
    }

    public async Task<IActionResult> ExportScoutsExcel(Guid? groupeId, DateTime? start, DateTime? end)
    {
        var q = _db.Scouts.Include(s => s.Groupe).AsQueryable();
        q = ApplyScoutsFilters(q, groupeId, start, end);

        var scouts = await q.OrderBy(s => s.Nom).ToListAsync();

        using var wb = new ClosedXML.Excel.XLWorkbook();
        var ws = wb.Worksheets.Add("Scouts");

        ws.Cell(1, 1).Value = "Matricule";
        ws.Cell(1, 2).Value = "Nom";
        ws.Cell(1, 3).Value = "Prénoms";
        ws.Cell(1, 4).Value = "Groupe";
        ws.Cell(1, 5).Value = "Statut";
        ws.Cell(1, 6).Value = "Date inscription";

        ws.Row(1).Style.Font.Bold = true;

        var row = 2;
        foreach (var s in scouts)
        {
            ws.Cell(row, 1).Value = s.Matricule;
            ws.Cell(row, 2).Value = s.Nom;
            ws.Cell(row, 3).Value = s.Prenoms;
            ws.Cell(row, 4).Value = s.Groupe?.Nom;
            ws.Cell(row, 5).Value = s.Statut;
            ws.Cell(row, 6).Value = s.CreatedAt.ToString("dd/MM/yyyy");
            row++;
        }

        ws.Columns().AdjustToContents();

        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        return File(ms.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "scouts_filtre.xlsx");
    }

    // Exports Cotisations
    public async Task<IActionResult> ExportCotisationsExcel(Guid? groupeId, string? periode, DateTime? start, DateTime? end)
    {
        var cotisations = await _db.Cotisations
            .Include(c => c.Scout)
            .Include(c => c.Groupe)
            .Where(c => 
                (!groupeId.HasValue || c.GroupeId == groupeId.Value) &&
                (string.IsNullOrEmpty(periode) || c.Periode == periode) &&
                (!start.HasValue || c.DateEnregistrement >= start.Value) &&
                (!end.HasValue || c.DateEnregistrement <= end.Value.AddDays(1)))
            .OrderByDescending(c => c.DateEnregistrement)
            .ToListAsync();

        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Cotisations");

        ws.Cell(1, 1).Value = "Scout";
        ws.Cell(1, 2).Value = "Groupe";
        ws.Cell(1, 3).Value = "Période";
        ws.Cell(1, 4).Value = "Montant Attendu";
        ws.Cell(1, 5).Value = "Montant Payé";
        ws.Cell(1, 6).Value = "Statut";
        ws.Cell(1, 7).Value = "Date Enregistrement";
        ws.Cell(1, 8).Value = "Date Paiement";

        ws.Row(1).Style.Font.Bold = true;

        var row = 2;
        foreach (var c in cotisations)
        {
            ws.Cell(row, 1).Value = $"{c.Scout?.Nom} {c.Scout?.Prenoms}";
            ws.Cell(row, 2).Value = c.Groupe?.Nom;
            ws.Cell(row, 3).Value = c.Periode;
            ws.Cell(row, 4).Value = c.MontantAttendu;
            ws.Cell(row, 5).Value = c.MontantPaye;
            ws.Cell(row, 6).Value = c.Statut.ToString();
            ws.Cell(row, 7).Value = c.DateEnregistrement.ToString("dd/MM/yyyy");
            ws.Cell(row, 8).Value = c.DatePaiement?.ToString("dd/MM/yyyy") ?? "";
            row++;
        }

        ws.Columns().AdjustToContents();

        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        return File(ms.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "cotisations.xlsx");
    }

    public async Task<IActionResult> ExportCotisationsCsv(Guid? groupeId, string? periode, DateTime? start, DateTime? end)
    {
        var cotisations = await _db.Cotisations
            .Include(c => c.Scout)
            .Include(c => c.Groupe)
            .Where(c => 
                (!groupeId.HasValue || c.GroupeId == groupeId.Value) &&
                (string.IsNullOrEmpty(periode) || c.Periode == periode) &&
                (!start.HasValue || c.DateEnregistrement >= start.Value) &&
                (!end.HasValue || c.DateEnregistrement <= end.Value.AddDays(1)))
            .OrderByDescending(c => c.DateEnregistrement)
            .ToListAsync();

        var csv = "Scout;Groupe;Période;Montant Attendu;Montant Payé;Statut;Date Enregistrement;Date Paiement\n" +
                  string.Join("\n", cotisations.Select(c =>
                      $"\"{c.Scout?.Nom} {c.Scout?.Prenoms}\";\"{c.Groupe?.Nom}\";{c.Periode};{c.MontantAttendu};{c.MontantPaye};{c.Statut};{c.DateEnregistrement:dd/MM/yyyy};{(c.DatePaiement?.ToString("dd/MM/yyyy") ?? "")}"
                  ));

        return File(System.Text.Encoding.UTF8.GetBytes(csv), "text/csv", "cotisations.csv");
    }

    // Exports Nominations
    public async Task<IActionResult> ExportNominationsExcel(Guid? groupeId, Guid? scoutId, DateTime? start, DateTime? end)
    {
        var nominations = await _db.Nominations
            .Include(n => n.Scout)
            .Include(n => n.Groupe)
            .Include(n => n.AutoriteValidation)
            .Where(n => 
                (!groupeId.HasValue || n.GroupeId == groupeId.Value) &&
                (!scoutId.HasValue || n.ScoutId == scoutId.Value) &&
                (!start.HasValue || n.DateDebut.ToDateTime(TimeOnly.MinValue) >= start.Value) &&
                (!end.HasValue || n.DateDebut.ToDateTime(TimeOnly.MinValue) <= end.Value.AddDays(1)))
            .OrderByDescending(n => n.DateDebut)
            .ToListAsync();

        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Nominations");

        ws.Cell(1, 1).Value = "Scout";
        ws.Cell(1, 2).Value = "Groupe";
        ws.Cell(1, 3).Value = "Poste";
        ws.Cell(1, 4).Value = "Fonction";
        ws.Cell(1, 5).Value = "Date Début";
        ws.Cell(1, 6).Value = "Date Fin";
        ws.Cell(1, 7).Value = "Statut";
        ws.Cell(1, 8).Value = "Autorité Validation";
        ws.Cell(1, 9).Value = "Date Création";

        ws.Row(1).Style.Font.Bold = true;

        var row = 2;
        foreach (var n in nominations)
        {
            ws.Cell(row, 1).Value = $"{n.Scout?.Nom} {n.Scout?.Prenoms}";
            ws.Cell(row, 2).Value = n.Groupe?.Nom;
            ws.Cell(row, 3).Value = n.Poste;
            ws.Cell(row, 4).Value = n.Fonction ?? "";
            ws.Cell(row, 5).Value = n.DateDebut.ToString("dd/MM/yyyy");
            ws.Cell(row, 6).Value = n.DateFin?.ToString("dd/MM/yyyy") ?? "";
            ws.Cell(row, 7).Value = n.Statut.ToString();
            ws.Cell(row, 8).Value = n.AutoriteValidation != null ? $"{n.AutoriteValidation.Nom} {n.AutoriteValidation.Prenoms}" : "";
            ws.Cell(row, 9).Value = n.CreatedAt.ToString("dd/MM/yyyy");
            row++;
        }

        ws.Columns().AdjustToContents();

        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        return File(ms.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "nominations.xlsx");
    }

    public async Task<IActionResult> ExportNominationsCsv(Guid? groupeId, Guid? scoutId, DateTime? start, DateTime? end)
    {
        var nominations = await _db.Nominations
            .Include(n => n.Scout)
            .Include(n => n.Groupe)
            .Include(n => n.AutoriteValidation)
            .Where(n => 
                (!groupeId.HasValue || n.GroupeId == groupeId.Value) &&
                (!scoutId.HasValue || n.ScoutId == scoutId.Value) &&
                (!start.HasValue || n.DateDebut.ToDateTime(TimeOnly.MinValue) >= start.Value) &&
                (!end.HasValue || n.DateDebut.ToDateTime(TimeOnly.MinValue) <= end.Value.AddDays(1)))
            .OrderByDescending(n => n.DateDebut)
            .ToListAsync();

        var csv = "Scout;Groupe;Poste;Fonction;Date Début;Date Fin;Statut;Autorité Validation;Date Création\n" +
                  string.Join("\n", nominations.Select(n =>
                      $"\"{n.Scout?.Nom} {n.Scout?.Prenoms}\";\"{n.Groupe?.Nom}\";\"{n.Poste}\";\"{n.Fonction ?? ""}\";{n.DateDebut:dd/MM/yyyy};{(n.DateFin?.ToString("dd/MM/yyyy") ?? "")};{n.Statut};\"{(n.AutoriteValidation != null ? $"{n.AutoriteValidation.Nom} {n.AutoriteValidation.Prenoms}" : "")}\";{n.CreatedAt:dd/MM/yyyy}"
                  ));

        return File(System.Text.Encoding.UTF8.GetBytes(csv), "text/csv", "nominations.csv");
    }

    public async Task<IActionResult> ExportCotisationsPdfOfficial(Guid? groupeId = null, string? periode = null, DateTime? start = null, DateTime? end = null)
    {
        var query = _db.Cotisations
            .Include(c => c.Scout)
            .Include(c => c.Groupe)
            .AsQueryable();

        if (groupeId.HasValue)
            query = query.Where(c => c.GroupeId == groupeId.Value);

        if (!string.IsNullOrEmpty(periode))
            query = query.Where(c => c.Periode == periode);

        if (start.HasValue)
            query = query.Where(c => c.DateEnregistrement >= start.Value);

        if (end.HasValue)
            query = query.Where(c => c.DateEnregistrement <= end.Value.AddDays(1));

        var cotisations = await query.OrderByDescending(c => c.DateEnregistrement).ToListAsync();

        var totalCotisations = cotisations.Count;
        var cotisationsPayees = cotisations.Count(c => c.Statut == Domain.Enums.StatutCotisation.PAYE);
        var cotisationsPartielles = cotisations.Count(c => c.Statut == Domain.Enums.StatutCotisation.PARTIEL);
        var cotisationsImpayees = cotisations.Count(c => c.Statut == Domain.Enums.StatutCotisation.IMPAYE);

        var montantTotalAttendu = cotisations.Sum(c => c.MontantAttendu);
        var montantTotalPaye = cotisations.Sum(c => c.MontantPaye);
        var tauxCotisation = montantTotalAttendu > 0 ? (montantTotalPaye / montantTotalAttendu * 100) : 0;

        var data = new CotisationsReportData
        {
            GeneratedAtUtc = DateTime.UtcNow,
            TotalCotisations = totalCotisations,
            TotalGroupes = await _db.Groupes.CountAsync(),
            CotisationsPayees = cotisationsPayees,
            CotisationsPartielles = cotisationsPartielles,
            CotisationsImpayees = cotisationsImpayees,
            MontantTotalAttendu = montantTotalAttendu,
            MontantTotalPaye = montantTotalPaye,
            TauxCotisation = tauxCotisation,
            ByGroupe = cotisations
                .GroupBy(c => c.Groupe?.Nom ?? "N/A")
                .OrderByDescending(g => g.Count())
                .ToDictionary(g => g.Key, g => g.Count()),
            ByStatut = cotisations
                .GroupBy(c => c.Statut.ToString())
                .OrderByDescending(g => g.Count())
                .ToDictionary(g => g.Key, g => g.Count())
        };

        // Logo depuis wwwroot/assets/logo.png
        byte[]? logoBytes = null;
        var env = HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>();
        var logoPath = Path.Combine(env.WebRootPath, "assets", "logo.png");
        if (System.IO.File.Exists(logoPath))
            logoBytes = await System.IO.File.ReadAllBytesAsync(logoPath);

        var doc = new CotisationsOfficialPdfDocument(data, cotisations, logoBytes);
        var bytes = doc.GeneratePdf();

        return File(bytes, "application/pdf", "rapport_cotisations_officiel.pdf");
    }

    public async Task<IActionResult> ExportNominationsPdfOfficial(Guid? groupeId = null, Guid? scoutId = null, DateTime? start = null, DateTime? end = null)
    {
        var query = _db.Nominations
            .Include(n => n.Scout)
            .Include(n => n.Groupe)
            .AsQueryable();

        if (groupeId.HasValue)
            query = query.Where(n => n.GroupeId == groupeId.Value);

        if (scoutId.HasValue)
            query = query.Where(n => n.ScoutId == scoutId.Value);

        if (start.HasValue)
            query = query.Where(n => n.DateDebut.ToDateTime(TimeOnly.MinValue) >= start.Value);

        if (end.HasValue)
            query = query.Where(n => n.DateDebut.ToDateTime(TimeOnly.MinValue) <= end.Value.AddDays(1));

        var nominations = await query.OrderByDescending(n => n.DateDebut).ToListAsync();

        var totalNominations = nominations.Count;
        var nominationsValidees = nominations.Count(n => n.Statut == Domain.Enums.StatutNomination.VALIDEE);
        var nominationsEnAttente = nominations.Count(n => n.Statut == Domain.Enums.StatutNomination.EN_ATTENTE || n.Statut == Domain.Enums.StatutNomination.SOUMISE);
        var nominationsRejetees = nominations.Count(n => n.Statut == Domain.Enums.StatutNomination.REJETEE);

        var data = new NominationsReportData
        {
            GeneratedAtUtc = DateTime.UtcNow,
            TotalNominations = totalNominations,
            TotalGroupes = await _db.Groupes.CountAsync(),
            NominationsValidees = nominationsValidees,
            NominationsEnAttente = nominationsEnAttente,
            NominationsRejetees = nominationsRejetees,
            ByGroupe = nominations
                .GroupBy(n => n.Groupe?.Nom ?? "N/A")
                .OrderByDescending(g => g.Count())
                .ToDictionary(g => g.Key, g => g.Count()),
            ByStatut = nominations
                .GroupBy(n => n.Statut.ToString())
                .OrderByDescending(g => g.Count())
                .ToDictionary(g => g.Key, g => g.Count()),
            ByPoste = nominations
                .GroupBy(n => n.Poste)
                .OrderByDescending(g => g.Count())
                .ToDictionary(g => g.Key, g => g.Count())
        };

        // Logo depuis wwwroot/assets/logo.png
        byte[]? logoBytes = null;
        var env = HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>();
        var logoPath = Path.Combine(env.WebRootPath, "assets", "logo.png");
        if (System.IO.File.Exists(logoPath))
            logoBytes = await System.IO.File.ReadAllBytesAsync(logoPath);

        var doc = new NominationsOfficialPdfDocument(data, nominations, logoBytes);
        var bytes = doc.GeneratePdf();

        return File(bytes, "application/pdf", "rapport_nominations_officiel.pdf");
    }
}
