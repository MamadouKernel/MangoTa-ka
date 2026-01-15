namespace MangoTaikaDistrict.Domain.Entities;

public class Media
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid AlbumId { get; set; }
    public Album Album { get; set; } = default!;

    public string MediaType { get; set; } = "Photo"; // Photo/Video
    public string FilePath { get; set; } = default!;
    public string? Caption { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
