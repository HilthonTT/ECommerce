namespace ECommerce.Modules.Users.Application.Abstractions.Identity;

public sealed record LimitedTokenClaims(Guid UserId, string Purpose);