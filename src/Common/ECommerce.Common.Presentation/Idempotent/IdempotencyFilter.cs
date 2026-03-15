using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.Common.Presentation.Idempotent;

public sealed class IdempotencyFilter(int cacheTimeInMinutes = IdempotencyHelper.DefaultCacheTimeInMinutes)
    : IEndpointFilter
{
    private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(cacheTimeInMinutes);

    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        if (!IdempotencyHelper.TryGetIdempotenceKey(
                context.HttpContext.Request, out Guid idempotenceKey))
        {
            return Results.BadRequest("Invalid or missing Idempotence-Key header");
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
            return new IdempotentResult(cached.StatusCode, cached.Value);
        }

        // Place a short-lived sentinel to block concurrent duplicates
        await IdempotencyHelper.SetProcessingSentinelAsync(cache, cacheKey, ct);

        try
        {
            object? result = await next(context);

            if (result is IStatusCodeHttpResult { StatusCode: >= 200 and < 300 } statusCodeResult
                and IValueHttpResult valueResult)
            {
                int statusCode = statusCodeResult.StatusCode ?? StatusCodes.Status200OK;
                IdempotentResponse response = new(statusCode, valueResult.Value);

                await IdempotencyHelper.CacheResponseAsync(
                    cache, cacheKey, response, _cacheDuration, ct);
            }

            return result;
        }
        catch
        {
            // Clean up the sentinel so the client can retry with the same key
            await IdempotencyHelper.RemoveSentinelAsync(cache, cacheKey, CancellationToken.None);
            throw;
        }
    }
}
