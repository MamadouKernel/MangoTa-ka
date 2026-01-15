namespace MangoTaikaDistrict.Infrastructure.Pdf;

public class ScoutsReportData
{
    public DateTime GeneratedAtUtc { get; set; }
    public int TotalScouts { get; set; }
    public int TotalGroupes { get; set; }

    public Dictionary<string, int> ByGroupe { get; set; } = new();
    public Dictionary<string, int> ByStatut { get; set; } = new();
    public Dictionary<string, int> BySexe { get; set; } = new(); // "M", "F", "N/A"
}
