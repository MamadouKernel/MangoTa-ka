using MangoTaikaDistrict.Domain.Enums;
using System.Security.Cryptography;

namespace MangoTaikaDistrict.Domain.Entities;

public class Album
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Titre { get; set; } = default!;
    public Visibilite Visibilite { get; set; } = Visibilite.PUBLIC;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<Media> Medias { get; set; } = new();
}
