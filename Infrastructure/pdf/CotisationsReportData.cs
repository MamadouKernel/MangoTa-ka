namespace MangoTaikaDistrict.Infrastructure.Pdf;

public class CotisationsReportData
{
    public DateTime GeneratedAtUtc { get; set; }
    public int TotalCotisations { get; set; }
    public int TotalGroupes { get; set; }
    public int CotisationsPayees { get; set; }
    public int CotisationsPartielles { get; set; }
    public int CotisationsImpayees { get; set; }
    public decimal MontantTotalAttendu { get; set; }
    public decimal MontantTotalPaye { get; set; }
    public decimal TauxCotisation { get; set; }

    public Dictionary<string, int> ByGroupe { get; set; } = new();
    public Dictionary<string, int> ByStatut { get; set; } = new();
    public Dictionary<string, decimal> ByGroupeMontant { get; set; } = new();
}
