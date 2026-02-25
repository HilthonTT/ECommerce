using ECommerce.Api.Middleware;

namespace ECommerce.Api.Extensions;

internal static class MiddlewareExtensions
{
    public static IApplicationBuilder UseUserContextEnrichment(this IApplicationBuilder app)
    {
        return app.UseMiddleware<UserContextEnrichmentMiddleware>();
    }
}
