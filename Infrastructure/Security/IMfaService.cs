namespace MangoTaikaDistrict.Infrastructure.Security;

public interface IMfaService
{
    string GenerateSecret();
    string GenerateQrCodeUri(string email, string secret);
    bool ValidateCode(string secret, string code);
}
