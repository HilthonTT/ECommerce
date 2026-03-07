using ECommerce.WebApp.Features.Authentication.Providers;
using ECommerce.WebApp.Features.Authentication.Services;
using ECommerce.WebApp.Features.Catalog.Services;
using Microsoft.AspNetCore.Components.Authorization;

namespace ECommerce.WebApp;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddHttpClient("ECommerceApi", client =>
        {
            client.BaseAddress = new Uri("https+http://ecommerce-api");
        });

        services.AddScoped<ICatalogService, CatalogService>();
        services.AddScoped<IAuthService, AuthService>();

        services.AddScoped<TokenAuthStateProvider>();
        services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<TokenAuthStateProvider>());

        return services; 
    }
}
