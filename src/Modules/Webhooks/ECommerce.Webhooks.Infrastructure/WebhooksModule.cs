using ECommerce.Common.Application.EventBus;
using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Application.Sorting;
using ECommerce.Common.Infrastructure.Database;
using ECommerce.Common.Presentation.Endpoints;
using ECommerce.Webhooks.Application.Abstractions.Authentication;
using ECommerce.Webhooks.Application.Abstractions.Data;
using ECommerce.Webhooks.Application.Abstractions.Webhooks;
using ECommerce.Webhooks.Application.Webhooks;
using ECommerce.Webhooks.Application.Webhooks.GetSubscriptions;
using ECommerce.Webhooks.Domain.Webhooks;
using ECommerce.Webhooks.Infrastructure.Authentication;
using ECommerce.Webhooks.Infrastructure.Database;
using ECommerce.Webhooks.Infrastructure.Inbox;
using ECommerce.Webhooks.Infrastructure.Outbox;
using ECommerce.Webhooks.Infrastructure.Webhooks;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ECommerce.Webhooks.Infrastructure;

public static class WebhooksModule
{
    public static IServiceCollection AddWebhooksModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDomainEventHandlers();
        services.AddIntegrationEventHandlers();

        services
            .AddInfrastructure(configuration)
            .AddEndpoints(Presentation.AssemblyReference.Assembly);

        services.AddSingleton<ISortMappingDefinition, SortMappingDefinition<WebhookSubscriptionResponse, WebhookSubscription>>(
            _ => WebhookSubscriptionMappings.SortMapping);

        return services;
    }
    private static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabase(configuration);

        services.AddScoped<IWebhookDispatcher, WebhookDispatcher>();

        services.AddScoped<IUserContext, UserContext>();

        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration) =>
        services
            .AddDbContext<WebhooksDbContext>(Postgres.StandardOptions(configuration, Schemas.Webhooks))
            .AddScoped<IDbContext>(sp => sp.GetRequiredService<WebhooksDbContext>())
            .AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<WebhooksDbContext>())
            .AddScoped<IWebhookSubscriptionRepository, WebhookSubcriptionRepository>();

    public static void ConfigureConsumers(IRegistrationConfigurator registrationConfigurator) =>
        Presentation.AssemblyReference.Assembly
            .GetTypes()
            .Where(type => type.IsAssignableTo(typeof(IIntegrationEventHandler)))
            .ToList()
            .ForEach(integrationEventHandlerType =>
            {
                var integrationEventType = integrationEventHandlerType
                    .GetInterfaces()
                    .Single(@interface => @interface.IsGenericType)
                    .GetGenericArguments()
                    .Single();

                registrationConfigurator.AddConsumer(typeof(IntegrationEventConsumer<>).MakeGenericType(integrationEventType));
            });

    private static void AddDomainEventHandlers(this IServiceCollection services) =>
        Application.AssemblyReference.Assembly
            .GetTypes()
            .Where(type => type.IsAssignableTo(typeof(IDomainEventHandler)))
            .ToList()
            .ForEach(domainEventHandlerType =>
            {
                services.TryAddScoped(domainEventHandlerType);

                var domainEventType = domainEventHandlerType
                    .GetInterfaces()
                    .Single(@interface => @interface.IsGenericType)
                    .GetGenericArguments()
                    .Single();

                var closedIdempotentHandlerType =
                    typeof(IdempotentDomainEventHandler<>).MakeGenericType(domainEventType);

                services.Decorate(domainEventHandlerType, closedIdempotentHandlerType);
            });

    private static void AddIntegrationEventHandlers(this IServiceCollection services) =>
        Presentation.AssemblyReference.Assembly
            .GetTypes()
            .Where(type => type.IsAssignableTo(typeof(IIntegrationEventHandler)))
            .ToList()
            .ForEach(integrationEventHandlerType =>
            {
                services.TryAddScoped(integrationEventHandlerType);

                var integrationEventType = integrationEventHandlerType
                    .GetInterfaces()
                    .Single(@interface => @interface.IsGenericType)
                    .GetGenericArguments()
                    .Single();

                var closedIdempotentHandlerType = typeof(IdempotentIntegrationEventHandler<>).MakeGenericType(integrationEventType);

                services.Decorate(integrationEventHandlerType, closedIdempotentHandlerType);
            });
}
