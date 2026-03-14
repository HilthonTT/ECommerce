namespace ECommerce.Web.Shared.DTOs.Authentication;

public sealed class LoginDto
{
    public required bool RequiresTwoFactor { get; init; }

    public string? LimitedToken { get; init; }

    public AccessTokensDto? AccessTokens { get; init; }
}
