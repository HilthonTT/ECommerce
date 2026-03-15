using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.Common.Presentation.Idempotent;

[AttributeUsage(AttributeTargets.Method)]
public sealed class IdempotentAttribute : Attribute, IAsyncActionFilter
{
    private readonly TimeSpan _cacheDuration;

    public IdempotentAttribute(
        int cacheTimeInMinutes = IdempotencyHelper.DefaultCacheTimeInMinutes)
    {
        _cacheDuration = TimeSpan.FromMinutes(cacheTimeInMinutes);
    }

    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        if (!IdempotencyHelper.TryGetIdempotenceKey(
                context.HttpContext.Request, out Guid idempotenceKey))
        {
            context.Result = new BadRequestObjectResult(
                "Invalid or missing Idempotence-Key header");
            return;
        }

        IDistributedCache cache = context.HttpContext
            .RequestServices.GetRequiredService<IDistributedCache>();

        string cacheKey = IdempotencyHelper.CacheKey(idempotenceKey);
        CancellationToken ct = context.HttpContext.RequestAborted;

        // Return the cached response if one already exists
        IdempotentResponse? cached = await IdempotencyHelper
            .TryGetCachedResponseAsync(cache, cacheKey, ct);

        if (cached is not null)
        {
            context.Result = new ObjectResult(cached.Value) { StatusCode = cached.StatusCode };
            return;
        }

        // Place a short-lived sentinel to block concurrent duplicates
        await IdempotencyHelper.SetProcessingSentinelAsync(cache, cacheKey, ct);

        try
        {
            ActionExecutedContext executedContext = await next();

            if (executedContext.Result is ObjectResult { StatusCode: >= 200 and < 300 } objectResult)
            {
                int statusCode = objectResult.StatusCode ?? StatusCodes.Status200OK;
                IdempotentResponse response = new(statusCode, objectResult.Value);

                await IdempotencyHelper.CacheResponseAsync(
                    cache, cacheKey, response, _cacheDuration, ct);
            }
        }
        catch
        {
            await IdempotencyHelper.RemoveSentinelAsync(cache, cacheKey, CancellationToken.None);
            throw;
        }
    }
}
