namespace ECommerce.Modules.Users.Application.Abstractions.Identity;

public interface ITokenService
{
    string GenerateLimitedToken(Guid userId, string purpose);

    LimitedTokenClaims? ValidateLimitedToken(string token, string expectedPurpose);
}
