namespace MangoTaikaDistrict.Application.Interfaces;

public interface IEmailService
{
    Task<bool> SendEmailAsync(string to, string subject, string body, string? from = null);
    Task<bool> SendContactEmailAsync(string nom, string email, string telephone, string message, string type = "Contact");
}
