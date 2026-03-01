using ECommerce.Common.Application.Caching;
using ECommerce.Common.Application.DTOs;
using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Domain;

namespace ECommerce.Modules.Catalog.Application.Catalog.GetItems.Decorators;

internal sealed class GetCatalogItemsQueryHandlerCacheDecorator(
    IQueryHandler<GetCatalogItemsQuery, PaginationResult<CatalogItemResponse>> inner,
    ICacheService cacheService)
    : IQueryHandler<GetCatalogItemsQuery, PaginationResult<CatalogItemResponse>>
{
    private static string CacheKey(GetCatalogItemsQuery q) =>
        $"catalog-items:page={q.Page}:size={q.PageSize}:q={q.SearchTerm}:sort={q.Sort}" +
        $":brand={q.CatalogBrandId}:type={q.CatalogTypeId}";

    public async Task<Result<PaginationResult<CatalogItemResponse>>> Handle(
        GetCatalogItemsQuery query,
        CancellationToken cancellationToken)
    {
        string key = CacheKey(query);

        var cached = await cacheService.GetAsync<PaginationResult<CatalogItemResponse>>(
            key,
            cancellationToken);

        if (cached is not null)
        {
            return cached;
        }

        var result = await inner.Handle(query, cancellationToken);

        if (result.IsSuccess)
        {
            await cacheService.SetAsync(
                key,
                result.Value,
                TimeSpan.FromMinutes(1), // shorter TTL — list data changes more frequently
                cancellationToken);
        }

        return result;
    }
}
