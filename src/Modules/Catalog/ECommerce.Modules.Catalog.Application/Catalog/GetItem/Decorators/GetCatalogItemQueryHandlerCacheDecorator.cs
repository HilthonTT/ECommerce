using ECommerce.Common.Application.Caching;
using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Domain;

namespace ECommerce.Modules.Catalog.Application.Catalog.GetItem.Decorators;

internal sealed class GetCatalogItemQueryHandlerCacheDecorator(
      IQueryHandler<GetCatalogItemQuery, CatalogItemResponse> inner,
      ICacheService cacheService)
     : IQueryHandler<GetCatalogItemQuery, CatalogItemResponse>
{
    private static string CacheKey(int id) => $"catalog-items:{id}";

    public async Task<Result<CatalogItemResponse>> Handle(
        GetCatalogItemQuery query, 
        CancellationToken cancellationToken)
    {
        CatalogItemResponse? cached = await cacheService.GetAsync<CatalogItemResponse>(
            CacheKey(query.Id),
            cancellationToken);

        if (cached is not null)
        {
            return cached;
        }

        Result<CatalogItemResponse> result = await inner.Handle(query, cancellationToken);
        if (result.IsSuccess)
        {
            await cacheService.SetAsync(
                CacheKey(query.Id),
                result.Value,
                TimeSpan.FromMinutes(5),
                cancellationToken);
        }

        return result;
    }
}
