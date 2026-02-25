using ECommerce.Common.Application.Exceptions;
using System.Security.Claims;

namespace ECommerce.Common.Infrastructure.Authentication;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal? principal)
    {
        var userId = principal?.FindFirstValue(CustomClaims.Sub);

        return Guid.TryParse(userId, out var parsedUserId)
            ? parsedUserId
            : throw new ECommerceException("User identifier is unavailable");
    }

    public static string GetIdentityId(this ClaimsPrincipal? principal)
    {
        return principal?.FindFirstValue(ClaimTypes.NameIdentifier) ??
               throw new ECommerceException("User identity is unavailable");
    }

    public static Guid GetUserIdOrEmpty(this ClaimsPrincipal? principal)
    {
        var userId = principal?.FindFirstValue(CustomClaims.Sub);
        return Guid.TryParse(userId, out var parsedUserId)
            ? parsedUserId
            : Guid.Empty;
    }

    public static HashSet<string> GetPermissions(this ClaimsPrincipal? principal)
    {
        var permissionClaims = principal?.FindAll(CustomClaims.Permission) ??
                               throw new ECommerceException("Permissions are unavailable");
        return permissionClaims.Select(claim => claim.Value).ToHashSet();
    }
}