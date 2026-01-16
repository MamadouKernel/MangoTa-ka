using MangoTaikaDistrict.Domain.Entities;

namespace MangoTaikaDistrict.Infrastructure.Services;

public interface IExcelImportService
{
    Task<(int success, int errors, List<string> errorMessages)> ImportScoutsAsync(Stream excelStream);
    Task<(int success, int errors, List<string> errorMessages)> ImportCotisationsAsync(Stream excelStream);
    Task<(int success, int errors, List<string> errorMessages)> ImportNominationsAsync(Stream excelStream);
}
