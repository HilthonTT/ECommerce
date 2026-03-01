using ECommerce.Common.Application.Behaviors;
using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Application.Sorting;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace ECommerce.Common.Application;

public static class ApplicationConfiguration
{
    public static IServiceCollection AddApplication(this IServiceCollection services, Assembly[] moduleAssemblies)
    {
        services.Scan(scan => scan.FromAssemblies(moduleAssemblies)
           .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)), publicOnly: false)
               .AsImplementedInterfaces()
               .WithScopedLifetime()
           .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<>)), publicOnly: false)
               .AsImplementedInterfaces()
               .WithScopedLifetime()
           .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<,>)), publicOnly: false)
               .AsImplementedInterfaces()
               .WithScopedLifetime());

        services.Decorate(typeof(IQueryHandler<,>), typeof(ValidationDecorator.QueryHandler<,>));
        services.Decorate(typeof(ICommandHandler<,>), typeof(ValidationDecorator.CommandHandler<,>));
        services.Decorate(typeof(ICommandHandler<>), typeof(ValidationDecorator.CommandBaseHandler<>));

        services.Decorate(typeof(IQueryHandler<,>), typeof(LoggingDecorator.QueryHandler<,>));
        services.Decorate(typeof(ICommandHandler<,>), typeof(LoggingDecorator.CommandHandler<,>));
        services.Decorate(typeof(ICommandHandler<>), typeof(LoggingDecorator.CommandBaseHandler<>));

        services.Scan(scan => scan.FromAssemblies(moduleAssemblies)
            .AddClasses(classes => classes.AssignableTo(typeof(IDomainEventHandler<>)), publicOnly: false)
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.AddValidatorsFromAssemblies(moduleAssemblies, includeInternalTypes: true);

        services.TryAddSingleton<ISortMappingProvider, SortMappingProvider>();

        return services;
    }
}
