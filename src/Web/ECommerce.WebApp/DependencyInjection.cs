using ECommerce.WebApp.Features.Catalog.Services;

namespace ECommerce.WebApp;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddHttpClient<ICatalogService, CatalogService>(o => o.BaseAddress = new("https+http://ecommerce-api"));

        return services; 
    }
}
