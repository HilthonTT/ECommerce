using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ECommerce.Common.Infrastructure.Authentication;

internal static class AuthenticationExtensions
{
    internal static IServiceCollection AddAuthenticationInternal(
        this IServiceCollection services)
    {
        //builder.Services.AddAuthentication()
        //    .AddKeycloakJwtBearer(
        //        serviceName: "ecommerce-keycloak",
        //        realm: "ecommerce",
        //        options =>
        //        {
        //            options.Audience = "account";
        //            options.RequireHttpsMetadata = false; // dev only
        //        });

        services.AddHttpContextAccessor();

        return services;
    }
}