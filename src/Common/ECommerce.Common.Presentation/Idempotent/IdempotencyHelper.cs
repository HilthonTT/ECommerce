using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Primitives;
using System.Text.Json;

namespace ECommerce.Common.Presentation.Idempotent;

internal static class IdempotencyHelper
{
    internal const string IdempotenceKeyHeader = "Idempotence-Key";
    internal const string CacheKeyPrefix = "Idempotent_";
    internal const string ProcessingSentinel = "__PROCESSING__";
    internal const int DefaultCacheTimeInMinutes = 60;

    internal static bool TryGetIdempotenceKey(HttpRequest request, out Guid idempotenceKey)
    {
        idempotenceKey = Guid.Empty;

        return request.Headers.TryGetValue(IdempotenceKeyHeader, out StringValues value)
            && Guid.TryParse(value, out idempotenceKey)
            && idempotenceKey != Guid.Empty;
    }

    internal static string CacheKey(Guid idempotenceKey) => $"{CacheKeyPrefix}{idempotenceKey}";

    internal static async Task<IdempotentResponse?> TryGetCachedResponseAsync(
        IDistributedCache cache,
        string cacheKey,
        CancellationToken cancellationToken = default)
    {
        string? cached = await cache.GetStringAsync(cacheKey, cancellationToken);

        if (cached is null)
        {
            return null;
        }

        if (cached == ProcessingSentinel)
        {
            throw new ConflictException(
                "A request with the same Idempotence-Key is currently being processed.");
        }

        return JsonSerializer.Deserialize<IdempotentResponse>(cached);
    }

    /// <summary>
    /// Sets a short-lived sentinel value to guard against concurrent duplicate requests.
    /// </summary>
    internal static async Task SetProcessingSentinelAsync(
        IDistributedCache cache,
        string cacheKey,
        CancellationToken cancellationToken = default)
    {
        await cache.SetStringAsync(
            cacheKey,
            ProcessingSentinel,
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
            },
            cancellationToken);
    }

    /// <summary>
    /// Caches the final idempotent response.
    /// </summary>
    internal static async Task CacheResponseAsync(
        IDistributedCache cache,
        string cacheKey,
        IdempotentResponse response,
        TimeSpan cacheDuration,
        CancellationToken cancellationToken = default)
    {
        await cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(response),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = cacheDuration
            },
            cancellationToken);
    }

    /// <summary>
    /// Removes the sentinel if the request failed so a retry can go through.
    /// </summary>
    internal static async Task RemoveSentinelAsync(
        IDistributedCache cache,
        string cacheKey,
        CancellationToken cancellationToken = default)
    {
        // Only remove if it's still the sentinel (not a real cached response)
        string? current = await cache.GetStringAsync(cacheKey, cancellationToken);
        if (current == ProcessingSentinel)
        {
            await cache.RemoveAsync(cacheKey, cancellationToken);
        }
    }
}
