using ECommerce.Common.Application.Caching;
using ECommerce.Common.Application.DTOs;
using ECommerce.Common.Application.Messaging;
using ECommerce.Modules.Catalog.Application.Catalog;
using ECommerce.Modules.Catalog.Application.Catalog.GetItem;
using ECommerce.Modules.Catalog.Application.Catalog.GetItem.Decorators;
using ECommerce.Modules.Catalog.Application.Catalog.GetItems;
using ECommerce.Modules.Catalog.Application.Catalog.GetItems.Decorators;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.Modules.Catalog.Application;

public static class CatalogApplicationConfiguration
{
    public static IServiceCollection AddCatalogApplication(this IServiceCollection services)
    {
        // Register the inner (already-decorated) handler under a key
        services.AddKeyedScoped<IQueryHandler<GetCatalogItemsQuery, PaginationResult<CatalogItemResponse>>,
            GetCatalogItemsQueryHandler>("inner-items");

        services.AddKeyedScoped<IQueryHandler<GetCatalogItemQuery, CatalogItemResponse>,
            GetCatalogItemQueryHandler>("inner-item");

        // Override the default registration with the cache decorator
        services.AddScoped<IQueryHandler<GetCatalogItemsQuery, PaginationResult<CatalogItemResponse>>>(sp =>
            new GetCatalogItemsQueryHandlerCacheDecorator(
                sp.GetRequiredKeyedService<IQueryHandler<GetCatalogItemsQuery, PaginationResult<CatalogItemResponse>>>("inner-items"),
                sp.GetRequiredService<ICacheService>()));

        services.AddScoped<IQueryHandler<GetCatalogItemQuery, CatalogItemResponse>>(sp =>
            new GetCatalogItemQueryHandlerCacheDecorator(
                sp.GetRequiredKeyedService<IQueryHandler<GetCatalogItemQuery, CatalogItemResponse>>("inner-item"),
                sp.GetRequiredService<ICacheService>()));

        return services;
    }
}
