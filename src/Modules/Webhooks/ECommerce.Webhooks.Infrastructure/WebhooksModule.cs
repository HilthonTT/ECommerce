using ECommerce.Common.Infrastructure.Database;
using ECommerce.Common.Presentation.Endpoints;
using ECommerce.Webhooks.Application.Abstractions.Data;
using ECommerce.Webhooks.Domain.Webhooks;
using ECommerce.Webhooks.Infrastructure.Database;
using ECommerce.Webhooks.Infrastructure.Webhooks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.Webhooks.Infrastructure;

public static class WebhooksModule
{
    public static IServiceCollection AddWebhooksModule(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddInfrastructure(configuration)
            .AddEndpoints(Presentation.AssemblyReference.Assembly);

        return services;
    }
    private static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabase(configuration);

        services.AddScoped<WebhookDispatcher>();

        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration) =>
        services
            .AddDbContext<WebhooksDbContext>(Postgres.StandardOptions(configuration, Schemas.Webhooks))
            .AddScoped<IDbContext>(sp => sp.GetRequiredService<WebhooksDbContext>())
            .AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<WebhooksDbContext>())
            .AddScoped<IWebhookSubscriptionRepository, WebhookSubcriptionRepository>();
}
