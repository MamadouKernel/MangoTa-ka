namespace MangoTaikaDistrict.Infrastructure.Storage;

public class FileStorageService : IFileStorageService
{
    private readonly IWebHostEnvironment _env;
    public FileStorageService(IWebHostEnvironment env) => _env = env;

    public async Task<(string filePath, string fileName, string contentType, long size)> SaveAsync(IFormFile file, string folder)
    {
        if (file == null || file.Length == 0) throw new InvalidOperationException("Fichier invalide.");

        var uploadsRoot = Path.Combine(_env.WebRootPath, "uploads", folder);
        Directory.CreateDirectory(uploadsRoot);

        var safeFileName = $"{Guid.NewGuid():N}_{Path.GetFileName(file.FileName)}";
        var fullPath = Path.Combine(uploadsRoot, safeFileName);

        await using var stream = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(stream);

        // chemin web (servi par static files)
        var webPath = $"/uploads/{folder}/{safeFileName}".Replace("\\", "/");

        return (webPath, file.FileName, file.ContentType, file.Length);
    }
}
