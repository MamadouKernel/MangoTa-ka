using OtpNet;

namespace MangoTaikaDistrict.Infrastructure.Security;

public class MfaService : IMfaService
{
    public string GenerateSecret()
    {
        var secret = KeyGeneration.GenerateRandomKey(20);
        return Base32Encoding.ToString(secret);
    }

    public string GenerateQrCodeUri(string email, string secret)
    {
        return $"otpauth://totp/MangoTaikaDistrict:{email}?secret={secret}&issuer=MangoTaikaDistrict";
    }

    public bool ValidateCode(string secret, string code)
    {
        try
        {
            var secretBytes = Base32Encoding.ToBytes(secret);
            var totp = new Totp(secretBytes);
            // Vérifier avec une tolérance de 1 pas de temps (30 secondes)
            return totp.VerifyTotp(code, out _, new VerificationWindow(1, 1));
        }
        catch
        {
            return false;
        }
    }
}
