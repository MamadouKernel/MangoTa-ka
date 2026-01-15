using ClosedXML.Excel;
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

    public async Task<IActionResult> Index()
    {
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

    public async Task<IActionResult> ExportScoutsExcel()
    {
        var scouts = await _db.Scouts.Include(s => s.Groupe)
            .OrderBy(s => s.Nom).ToListAsync();

        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Scouts");

        // Header
        ws.Cell(1, 1).Value = "Matricule";
        ws.Cell(1, 2).Value = "Nom";
        ws.Cell(1, 3).Value = "Prénoms";
        ws.Cell(1, 4).Value = "Groupe";
        ws.Cell(1, 5).Value = "Statut";

        ws.Row(1).Style.Font.Bold = true;

        // Data
        var row = 2;
        foreach (var s in scouts)
        {
            ws.Cell(row, 1).Value = s.Matricule;
            ws.Cell(row, 2).Value = s.Nom;
            ws.Cell(row, 3).Value = s.Prenoms;
            ws.Cell(row, 4).Value = s.Groupe?.Nom;
            ws.Cell(row, 5).Value = s.Statut;
            row++;
        }

        ws.Columns().AdjustToContents();

        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        var bytes = ms.ToArray();

        return File(bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "scouts.xlsx");
    }

    public async Task<IActionResult> ExportScoutsPdf()
    {
        var scouts = await _db.Scouts.Include(s => s.Groupe)
            .OrderBy(s => s.Nom).ToListAsync();

        var doc = new ScoutsPdfDocument(scouts);
        var bytes = doc.GeneratePdf();

        return File(bytes, "application/pdf", "scouts.pdf");
    }

    public async Task<IActionResult> ExportScoutsPdfOfficial()
    {
        var scouts = await _db.Scouts.Include(s => s.Groupe)
            .OrderBy(s => s.Nom).ToListAsync();

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


}
