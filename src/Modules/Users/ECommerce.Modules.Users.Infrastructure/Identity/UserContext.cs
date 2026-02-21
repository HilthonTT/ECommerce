using ECommerce.Common.Application.Exceptions;
using ECommerce.Common.Infrastructure.Authentication;
using ECommerce.Modules.Users.Application.Abstractions.Identity;
using Microsoft.AspNetCore.Http;

namespace ECommerce.Modules.Users.Infrastructure.Identity;

internal sealed class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    public Guid UserId => httpContextAccessor.HttpContext?.User.GetUserId() ??
        throw new ECommerceException("User identifier is unavailable");
}
