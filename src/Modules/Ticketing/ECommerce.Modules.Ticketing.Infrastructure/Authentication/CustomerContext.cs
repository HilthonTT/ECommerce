using ECommerce.Common.Application.Exceptions;
using ECommerce.Common.Infrastructure.Authentication;
using ECommerce.Modules.Ticketing.Application.Abstractions.Authentication;
using Microsoft.AspNetCore.Http;

namespace ECommerce.Modules.Ticketing.Infrastructure.Authentication;

internal sealed class CustomerContext(IHttpContextAccessor httpContextAccessor) : ICustomerContext
{
    public Guid CustomerId => httpContextAccessor.HttpContext?.User.GetUserId() ??
        throw new ECommerceException("User identifier is unavailable");
}
