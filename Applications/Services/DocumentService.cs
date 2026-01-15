using MangoTaikaDistrict.Domain.Entities;
using MangoTaikaDistrict.Infrastructure.Data;
using MangoTaikaDistrict.Infrastructure.Storage;

namespace MangoTaikaDistrict.Application.Services;

public class DocumentService
{
    private readonly AppDbContext _db;
    private readonly IFileStorageService _storage;

    public DocumentService(AppDbContext db, IFileStorageService storage)
    {
        _db = db;
        _storage = storage;
    }

    public async Task UploadAsync(Guid uploadedById, string ownerType, Guid ownerId, string docType, IFormFile file)
    {
        var (path, fileName, contentType, size) = await _storage.SaveAsync(file, ownerType.ToLowerInvariant());

        _db.Documents.Add(new Document
        {
            UploadedById = uploadedById,
            OwnerType = ownerType,
            OwnerId = ownerId,
            DocType = docType,
            FilePath = path,
            FileName = fileName,
            ContentType = contentType,
            SizeBytes = size,
            Statut = "En attente"
        });

        await _db.SaveChangesAsync();
    }
}
