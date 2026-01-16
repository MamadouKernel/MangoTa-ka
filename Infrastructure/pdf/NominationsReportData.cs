namespace MangoTaikaDistrict.Infrastructure.Pdf;

public class NominationsReportData
{
    public DateTime GeneratedAtUtc { get; set; }
    public int TotalNominations { get; set; }
    public int TotalGroupes { get; set; }
    public int NominationsValidees { get; set; }
    public int NominationsEnAttente { get; set; }
    public int NominationsRejetees { get; set; }

    public Dictionary<string, int> ByGroupe { get; set; } = new();
    public Dictionary<string, int> ByStatut { get; set; } = new();
    public Dictionary<string, int> ByPoste { get; set; } = new();
}
