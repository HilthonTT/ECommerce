using ECommerce.Common.Application.EventBus;
using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Infrastructure.Database;
using ECommerce.Common.Presentation.Endpoints;
using ECommerce.Modules.Catalog.Application.Abstractions.AI;
using ECommerce.Modules.Catalog.Application.Abstractions.Data;
using ECommerce.Modules.Catalog.Domain.Catalog;
using ECommerce.Modules.Catalog.Infrastructure.AI;
using ECommerce.Modules.Catalog.Infrastructure.Catalog;
using ECommerce.Modules.Catalog.Infrastructure.Database;
using ECommerce.Modules.Catalog.Infrastructure.Inbox;
using ECommerce.Modules.Catalog.Infrastructure.Outbox;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ECommerce.Modules.Catalog.Infrastructure;

public static class CatalogModule
{
    public static IServiceCollection AddTicketingModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDomainEventHandlers();
        services.AddIntegrationEventHandlers();

        services
            .AddInfrastructure(configuration)
            .AddEndpoints(Presentation.AssemblyReference.Assembly);

        return services;
    }

    private static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration) =>
        services
            .Configure<CatalogOptions>(configuration.GetSection("Catalog:Options"))
            .AddScoped<ICatalogAI, CatalogAI>()
            .AddDatabase(configuration)
            .AddOutbox(configuration)
            .AddInbox(configuration);

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration) =>
        services
            .AddDbContext<CatalogDbContext>(Postgres.StandardOptions(configuration, Schemas.Catalog))
            .AddScoped<IUnitOfWork>(serviceProvider => serviceProvider.GetRequiredService<CatalogDbContext>())
            .AddScoped<IDbContext>(serviceProvider => serviceProvider.GetRequiredService<CatalogDbContext>())
            .AddScoped<ICatalogItemRepository, CatalogItemRepository>()
            .AddScoped<IDbSeeder<CatalogDbContext>, CatalogDbContextSeeder>();

    private static IServiceCollection AddOutbox(this IServiceCollection services, IConfiguration configuration) =>
        services
            .Configure<OutboxOptions>(configuration.GetSection("Catalog:Outbox"))
            .ConfigureOptions<ConfigureProcessOutboxJob>();

    private static IServiceCollection AddInbox(this IServiceCollection services, IConfiguration configuration) =>
        services
            .Configure<InboxOptions>(configuration.GetSection("Catalog:Inbox"))
            .ConfigureOptions<ConfigureProcessInboxJob>();

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
