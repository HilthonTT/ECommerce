namespace ECommerce.Modules.Users.Application.Abstractions.TwoFactor;

public interface ITwoFactorService
{
    (string base32Secret, byte[] qrCodeImage) GenerateSetup(string issuer, string userIdentifier);

    bool VerifyCode(string base32Secret, string code, out long timeStepMatched);

    List<string> GenerateRecoveryCodes(int count = 8);

    bool VerifyRecoveryCode(string code, string hash);
}
