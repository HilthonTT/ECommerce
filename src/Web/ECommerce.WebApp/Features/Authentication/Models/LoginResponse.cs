namespace ECommerce.WebApp.Features.Authentication.Models;

public sealed record LoginResponse(
    bool RequiresTwoFactor,
    string? LimitedToken,
    AccessTokensResponse? AccessTokens);
