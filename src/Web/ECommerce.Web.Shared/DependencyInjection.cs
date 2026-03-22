using ECommerce.Web.Shared.Services.Authentication;
using ECommerce.Web.Shared.Services.Authentication.Interfaces;
using ECommerce.Web.Shared.Services.Cart;
using ECommerce.Web.Shared.Services.Cart.Interfaces;
using ECommerce.Web.Shared.Services.Catalog;
using ECommerce.Web.Shared.Services.Catalog.Interfaces;
using ECommerce.Web.Shared.Services.Common;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.Web.Shared;

public static class DependencyInjection
{
    public static IServiceCollection AddSharedServices(this IServiceCollection services)
    {
        services.AddScoped<TokenStore>();
        services.AddScoped<TokenAuthStateProvider>();
        services.AddScoped<AuthenticationStateProvider>(sp =>
            sp.GetRequiredService<TokenAuthStateProvider>());

        services.AddHttpClient(HttpClientFactoryNames.Default, client =>
        {
            client.BaseAddress = new Uri("https+http://ecommerce-api");
        });

        services.AddScoped<AuthenticatedHttpClient>();

        services.AddScoped<ICatalogService, CatalogService>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();

        return services;
    }
}
