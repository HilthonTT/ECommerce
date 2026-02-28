namespace ECommerce.Modules.Users.Application.TwoFactor.Setup;

public sealed record SetupTwoFactorResponse(byte[] QrCodeImage, string ManualEntryKey);