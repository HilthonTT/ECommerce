using ECommerce.Common.Application.EventBus;
using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Application.Sorting;
using ECommerce.Common.Infrastructure.Database;
using ECommerce.Common.Presentation.Endpoints;
using ECommerce.Modules.Ticketing.Application.Abstractions.Authentication;
using ECommerce.Modules.Ticketing.Application.Abstractions.Data;
using ECommerce.Modules.Ticketing.Application.Tickets;
using ECommerce.Modules.Ticketing.Domain.Carts;
using ECommerce.Modules.Ticketing.Domain.Customers;
using ECommerce.Modules.Ticketing.Domain.Messages;
using ECommerce.Modules.Ticketing.Domain.Orders;
using ECommerce.Modules.Ticketing.Domain.Products;
using ECommerce.Modules.Ticketing.Domain.Tickets;
using ECommerce.Modules.Ticketing.Infrastructure.AI;
using ECommerce.Modules.Ticketing.Infrastructure.Authentication;
using ECommerce.Modules.Ticketing.Infrastructure.Carts;
using ECommerce.Modules.Ticketing.Infrastructure.Customers;
using ECommerce.Modules.Ticketing.Infrastructure.Database;
using ECommerce.Modules.Ticketing.Infrastructure.Inbox;
using ECommerce.Modules.Ticketing.Infrastructure.Messages;
using ECommerce.Modules.Ticketing.Infrastructure.Orders;
using ECommerce.Modules.Ticketing.Infrastructure.Outbox;
using ECommerce.Modules.Ticketing.Infrastructure.Products;
using ECommerce.Modules.Ticketing.Infrastructure.Tickets;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using static ECommerce.Modules.Ticketing.Application.Tickets.GetTickets.GetTicketsResponse;

namespace ECommerce.Modules.Ticketing.Infrastructure;

public static class TicketingModule
{
    public static IServiceCollection AddTicketingModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDomainEventHandlers();
        services.AddIntegrationEventHandlers();

        services
            .AddInfrastructure(configuration)
            .AddEndpoints(Presentation.AssemblyReference.Assembly);

        services.AddSingleton<ISortMappingDefinition, SortMappingDefinition<GetTicketResponseItem, Ticket>>(_ => TicketMappings.SortMapping);

        return services;
    }

    private static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddSingleton<ICartService, CartService>()
            .AddDatabase(configuration)
            .AddOutbox(configuration)
            .AddInbox(configuration);

        services.AddScoped<ICustomerContext, CustomerContext>();

        services.AddHttpClient<PythonInferenceClient>(c => c.BaseAddress = new Uri("http://python-inference"));

        return services;
    }
        

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration) =>
        services
            .AddDbContext<TicketingDbContext>(Postgres.StandardOptions(configuration, Schemas.Ticketing))
            .AddScoped<IUnitOfWork>(serviceProvider => serviceProvider.GetRequiredService<TicketingDbContext>())
            .AddScoped<IDbContext>(serviceProvider => serviceProvider.GetRequiredService<TicketingDbContext>())
            .AddScoped<ICustomerRepository, CustomerRepository>()
            .AddScoped<IProductRepository, ProductRepository>()
            .AddScoped<IMessageRepository, MessageRepository>()
            .AddScoped<IOrderRepository, OrderRepository>()
            .AddScoped<ITicketRepository, TicketRepository>();

    private static IServiceCollection AddOutbox(this IServiceCollection services, IConfiguration configuration) =>
        services
            .Configure<OutboxOptions>(configuration.GetSection("Ticketing:Outbox"))
            .ConfigureOptions<ConfigureProcessOutboxJob>();

    private static IServiceCollection AddInbox(this IServiceCollection services, IConfiguration configuration) =>
        services
            .Configure<InboxOptions>(configuration.GetSection("Ticketing:Inbox"))
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
