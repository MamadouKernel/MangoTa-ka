namespace MangoTaikaDistrict.Domain.Entities;

public class Document
{
    public Guid Id { get; set; } = Guid.NewGuid();

    // polymorphisme simple: USER/SCOUT/GROUPE/ACTIVITE/TICKET/DEMANDE
    public string OwnerType { get; set; } = default!;
    public Guid OwnerId { get; set; }

    public string DocType { get; set; } = default!;

    public string FilePath { get; set; } = default!;
    public string FileName { get; set; } = default!;
    public string ContentType { get; set; } = default!;
    public long SizeBytes { get; set; }

    public string Statut { get; set; } = "En attente";

    public Guid UploadedById { get; set; }
    public Utilisateur UploadedBy { get; set; } = default!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
