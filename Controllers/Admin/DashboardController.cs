using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MangoTaikaDistrict.Domain.Enums;
using MangoTaikaDistrict.Infrastructure.Data;
using MangoTaikaDistrict.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MangoTaikaDistrict.Controllers.Admin;

[Authorize]
[Area("Admin")]
public class DashboardController : Controller
{
    private readonly AppDbContext _db;
    private readonly IUtilisateurRepository _users;
    private readonly IDemandeRgpdRepository _rgpd;
    private readonly IAscciStatusRepository _ascci;

    public DashboardController(AppDbContext db, IUtilisateurRepository users, IDemandeRgpdRepository rgpd, IAscciStatusRepository ascci)
    {
        _db = db;
        _users = users;
        _rgpd = rgpd;
        _ascci = ascci;
    }

    public async Task<IActionResult> Index(Guid? groupeId, DateTime? start, DateTime? end)
    {
        // Filtres par groupe et période
        ViewBag.Groupes = await _db.Groupes.OrderBy(g => g.Nom).ToListAsync();
        ViewBag.SelectedGroupeId = groupeId;
        
        var scoutsQuery = _db.Scouts.AsQueryable();
        var cotisationsQuery = _db.Cotisations.AsQueryable();

        if (groupeId.HasValue)
        {
            scoutsQuery = scoutsQuery.Where(s => s.GroupeId == groupeId.Value);
            cotisationsQuery = cotisationsQuery.Where(c => c.GroupeId == groupeId.Value);
        }

        if (start.HasValue)
        {
            scoutsQuery = scoutsQuery.Where(s => s.CreatedAt >= start.Value);
            cotisationsQuery = cotisationsQuery.Where(c => c.DateEnregistrement >= start.Value);
        }

        if (end.HasValue)
        {
            var endInclusive = end.Value.Date.AddDays(1);
            scoutsQuery = scoutsQuery.Where(s => s.CreatedAt < endInclusive);
            cotisationsQuery = cotisationsQuery.Where(c => c.DateEnregistrement < endInclusive);
        }

        var totalScouts = await scoutsQuery.CountAsync();
        var totalGroupes = await _db.Groupes.CountAsync();
        var totalActivites = await _db.Activites.CountAsync();
        var totalTickets = await _db.Tickets.CountAsync();

        // Statistiques cotisations (filtrées)
        var totalCotisations = await cotisationsQuery.CountAsync();
        var cotisationsPayees = await cotisationsQuery.CountAsync(c => c.Statut == StatutCotisation.PAYE);
        var cotisationsPartielles = await cotisationsQuery.CountAsync(c => c.Statut == StatutCotisation.PARTIEL);
        var cotisationsImpayees = await cotisationsQuery.CountAsync(c => c.Statut == StatutCotisation.IMPAYE);

        var montantTotalAttendu = await cotisationsQuery.SumAsync(c => c.MontantAttendu);
        var montantTotalPaye = await cotisationsQuery.SumAsync(c => c.MontantPaye);
        var tauxCotisation = totalCotisations > 0 ? (montantTotalPaye / montantTotalAttendu * 100) : 0;

        // Répartition par sexe (filtrée)
        var repartitionSexe = await scoutsQuery
            .Where(s => !string.IsNullOrEmpty(s.Sexe))
            .GroupBy(s => s.Sexe)
            .Select(g => new { Sexe = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Sexe ?? "N/A", x => x.Count);

        // Répartition par statut scout (filtrée)
        var repartitionStatut = await scoutsQuery
            .GroupBy(s => s.Statut)
            .Select(g => new { Statut = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Statut ?? "N/A", x => x.Count);

        // Top 5 groupes
        var topGroupes = await _db.Scouts
            .Include(s => s.Groupe)
            .Where(s => s.Groupe != null)
            .GroupBy(s => s.Groupe!.Nom)
            .Select(g => new { Groupe = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(5)
            .ToDictionaryAsync(x => x.Groupe ?? "N/A", x => x.Count);

        // Évolution cotisations par mois (6 derniers mois)
        var evolutionCotisations = new Dictionary<string, decimal>();
        for (int i = 5; i >= 0; i--)
        {
            var date = DateTime.UtcNow.AddMonths(-i);
            var mois = date.ToString("yyyy-MM");
            var montant = await _db.Cotisations
                .Where(c => c.DateEnregistrement.Year == date.Year && c.DateEnregistrement.Month == date.Month)
                .SumAsync(c => (decimal?)c.MontantPaye) ?? 0;
            evolutionCotisations[date.ToString("MMM yyyy")] = montant;
        }

        ViewBag.TotalScouts = totalScouts;
        ViewBag.TotalGroupes = totalGroupes;
        ViewBag.TotalActivites = totalActivites;
        ViewBag.TotalTickets = totalTickets;
        ViewBag.TotalCotisations = totalCotisations;
        ViewBag.CotisationsPayees = cotisationsPayees;
        ViewBag.CotisationsPartielles = cotisationsPartielles;
        ViewBag.CotisationsImpayees = cotisationsImpayees;
        ViewBag.MontantTotalAttendu = montantTotalAttendu;
        ViewBag.MontantTotalPaye = montantTotalPaye;
        ViewBag.TauxCotisation = tauxCotisation;
        ViewBag.RepartitionSexe = repartitionSexe;
        ViewBag.RepartitionStatut = repartitionStatut;
        ViewBag.TopGroupes = topGroupes;
        ViewBag.EvolutionCotisations = evolutionCotisations;
        ViewBag.Start = start?.ToString("yyyy-MM-dd") ?? "";
        ViewBag.End = end?.ToString("yyyy-MM-dd") ?? "";

        // Alertes pour les admins
        if (User.IsInRole("ADMIN") || User.IsInRole("GESTIONNAIRE"))
        {
            var pendingUsers = await _users.GetPendingValidationAsync();
            var pendingRgpd = await _rgpd.GetPendingAsync();
            var expiredAscci = await _ascci.GetExpiredAsync();
            var expiringSoonAscci = await _ascci.GetExpiringSoonAsync();

            ViewBag.PendingUsersCount = pendingUsers.Count;
            ViewBag.PendingRgpdCount = pendingRgpd.Count;
            ViewBag.ExpiredAscciCount = expiredAscci.Count;
            ViewBag.ExpiringSoonAscciCount = expiringSoonAscci.Count;
        }

        return View();
    }
}
