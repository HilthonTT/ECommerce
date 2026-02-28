using ECommerce.Modules.Users.Application.Abstractions.Identity;

namespace ECommerce.Modules.Users.Application.Authentication.LoginUser;

public sealed record LoginResponse(
    bool RequiresTwoFactor,
    string? LimitedToken,
    AccessTokensResponse? AccessTokens);