using ECommerce.Common.Application.Exceptions;
using ECommerce.Common.Infrastructure.Authentication;
using ECommerce.Webhooks.Application.Abstractions.Authentication;
using Microsoft.AspNetCore.Http;

namespace ECommerce.Webhooks.Infrastructure.Authentication;

internal sealed class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    public Guid UserId => httpContextAccessor.HttpContext?.User.GetUserId() ??
        throw new ECommerceException("User identifier is unavailable");
}
