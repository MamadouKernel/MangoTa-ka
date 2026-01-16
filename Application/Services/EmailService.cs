using MangoTaikaDistrict.Application.Interfaces;
using System.Net;
using System.Net.Mail;

namespace MangoTaikaDistrict.Application.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<bool> SendEmailAsync(string to, string subject, string body, string? from = null)
    {
        try
        {
            var smtpHost = _configuration["Email:SmtpHost"] ?? "smtp.gmail.com";
            var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
            var smtpUser = _configuration["Email:SmtpUser"];
            var smtpPassword = _configuration["Email:SmtpPassword"];
            var defaultFrom = _configuration["Email:From"] ?? "noreply@mangotaika.ci";

            if (string.IsNullOrEmpty(smtpUser) || string.IsNullOrEmpty(smtpPassword))
            {
                _logger.LogWarning("Configuration email SMTP manquante. L'email ne sera pas envoyé.");
                // En développement, on log simplement
                _logger.LogInformation("Email qui aurait été envoyé à {To}: {Subject}", to, subject);
                return false;
            }

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(smtpUser, smtpPassword)
            };

            using var message = new MailMessage
            {
                From = new MailAddress(from ?? defaultFrom),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            message.To.Add(to);

            await client.SendMailAsync(message);
            _logger.LogInformation("Email envoyé avec succès à {To}", to);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'envoi de l'email à {To}", to);
            return false;
        }
    }

    public async Task<bool> SendContactEmailAsync(string nom, string email, string telephone, string message, string type = "Contact")
    {
        var contactEmail = _configuration["Email:ContactEmail"] ?? "contact@mangotaika.ci";
        
        var subject = type switch
        {
            "Contact" => $"Contact - Message de {nom}",
            "Suggestion" => $"Suggestion/Commentaire - Message de {nom}",
            _ => $"Message de {nom}"
        };

        var body = $@"
<html>
<body style='font-family: Arial, sans-serif;'>
    <h2>{subject}</h2>
    <p><strong>Nom:</strong> {nom}</p>
    <p><strong>Email:</strong> {email}</p>
    <p><strong>Téléphone:</strong> {telephone}</p>
    <hr/>
    <p><strong>Message:</strong></p>
    <p>{message.Replace("\n", "<br/>")}</p>
</body>
</html>";

        return await SendEmailAsync(contactEmail, subject, body);
    }
}
