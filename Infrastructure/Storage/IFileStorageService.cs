namespace MangoTaikaDistrict.Infrastructure.Storage;

public interface IFileStorageService
{
    Task<(string filePath, string fileName, string contentType, long size)> SaveAsync(IFormFile file, string folder);
}
