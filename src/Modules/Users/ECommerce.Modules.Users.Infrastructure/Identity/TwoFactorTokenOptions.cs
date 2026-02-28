namespace ECommerce.Modules.Users.Infrastructure.Identity;

public sealed class TwoFactorTokenOptions
{
    public const string SectionName = "TwoFactorToken";

    public string SigningKey { get; init; } = string.Empty;
    public string Issuer { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public int ExpiryMinutes { get; init; } = 5;
}