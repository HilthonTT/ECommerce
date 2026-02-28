using ECommerce.Modules.Users.Application.Abstractions.TwoFactor;
using OtpNet;
using QRCoder;
using System.Security.Cryptography;

namespace ECommerce.Modules.Users.Infrastructure.TwoFactor;

internal sealed class TwoFactorService : ITwoFactorService
{
    public List<string> GenerateRecoveryCodes(int count = 8)
    {
        var codes = new List<string>(count);

        for (int i = 0; i < count; i++)
        {
            byte[] bytes = RandomNumberGenerator.GetBytes(5);
            // Format as XXXXX-XXXXX for readability
            string hex = Convert.ToHexString(bytes).ToLower();
            codes.Add($"{hex[..5]}-{hex[5..]}");
        }

        return codes;
    }

    public (string base32Secret, byte[] qrCodeImage) GenerateSetup(string issuer, string userIdentifier)
    {
        byte[] secretKey = KeyGeneration.GenerateRandomKey(); // 20 bytes, SHA-1 compatible
        string base32Secret = Base32Encoding.ToString(secretKey);

        string escapedIssuer = Uri.EscapeDataString(issuer);
        string escapedUser = Uri.EscapeDataString(userIdentifier);

        string otpUri =
            $"otpauth://totp/{escapedIssuer}:{escapedUser}" +
            $"?secret={base32Secret}" +
            $"&issuer={escapedIssuer}" +
            $"&digits=6" +
            $"&period=30";

        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(otpUri, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(qrCodeData);
        byte[] qrCodeImage = qrCode.GetGraphic(10);

        return (base32Secret, qrCodeImage);
    }

    public bool VerifyCode(string base32Secret, string code, out long timeStepMatched)
    {
        byte[] secretKey = Base32Encoding.ToBytes(base32Secret);
        var totp = new Totp(secretKey);

        return totp.VerifyTotp(
            code,
            out timeStepMatched,
            VerificationWindow.RfcSpecifiedNetworkDelay);
    }

    public bool VerifyRecoveryCode(string code, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(code, hash);
    }
}
