using ECommerce.Common.Infrastructure.Authentication;
using System.Diagnostics;

namespace ECommerce.Api.Middleware;

internal sealed class UserContextEnrichmentMiddleware(RequestDelegate next, ILogger<UserContextEnrichmentMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context, IHttpContextAccessor httpContextAccessor)
    {
        Guid userId = httpContextAccessor.HttpContext?.User.GetUserIdOrEmpty() ?? Guid.Empty;

        if (userId == Guid.Empty)
        {
            Activity.Current?.SetTag("user.id", userId);

            var state = new Dictionary<string, object>
            {
                ["UserId"] = userId
            };

            using (logger.BeginScope(state))
            {
                await next(context);
            }
        }
        else
        {
            await next(context);
        }
    }
}
