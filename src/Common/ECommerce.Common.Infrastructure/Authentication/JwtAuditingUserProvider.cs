using ECommerce.Common.Application.Exceptions;
using ECommerce.Common.Infrastructure.Auditing;
using Microsoft.AspNetCore.Http;

namespace ECommerce.Common.Infrastructure.Authentication;

public sealed class JwtAuditingUserProvider(IHttpContextAccessor httpContextAccessor) : IAuditingUserProvider
{
    private const string DefaultUser = "Unknown User";

    public string GetUserId()
    {
        try
        {
            return httpContextAccessor.HttpContext?.User.GetUserId().ToString() ?? DefaultUser;
        }
        catch (ECommerceException)
        {
            return DefaultUser;
        }
    }
}