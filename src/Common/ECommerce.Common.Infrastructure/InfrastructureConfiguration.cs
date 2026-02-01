using ECommerce.Common.Application.Caching;
using ECommerce.Common.Application.Clock;
using ECommerce.Common.Application.Data;
using ECommerce.Common.Application.EventBus;
using ECommerce.Common.Infrastructure.Auditing;
using ECommerce.Common.Infrastructure.Authentication;
using ECommerce.Common.Infrastructure.Authorization;
using ECommerce.Common.Infrastructure.Caching;
using ECommerce.Common.Infrastructure.Clock;
using ECommerce.Common.Infrastructure.Data;
using ECommerce.Common.Infrastructure.Outbox;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
using Quartz;
using StackExchange.Redis;
using System.Data.Common;

namespace ECommerce.Common.Infrastructure;

public static class InfrastructureConfiguration
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        Action<IRegistrationConfigurator>[] moduleConfigureConsumers,
        string databaseConnectionString,
        string cacheConnectionString)
    {
        services.AddAuthorizationInternal();
        services.AddAuthenticationInternal();

        services.AddAuditing();

        var npgsqlDataSource = new NpgsqlDataSourceBuilder(databaseConnectionString).Build();
        services.TryAddSingleton(npgsqlDataSource);

        services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();

        services.TryAddSingleton<InsertOutboxMessagesInterceptor>();

        services.TryAddSingleton<IDateTimeProvider, DateTimeProvider>();

        services.AddQuartz(configurator =>
        {
            var scheduler = Guid.NewGuid();
            configurator.SchedulerId = $"default-id-{scheduler}";
            configurator.SchedulerName = $"default-name-{scheduler}";
        });

        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

        try
        {
            IConnectionMultiplexer connectionMultiplexer = ConnectionMultiplexer.Connect(cacheConnectionString);
            services.TryAddSingleton(connectionMultiplexer);

            services.AddStackExchangeRedisCache(options =>
            {
                options.ConnectionMultiplexerFactory = () => Task.FromResult(connectionMultiplexer);
            });
        }
        catch
        {
            // HACK: Allows application to run without a Redis server. This is useful when, for example, generating a database migration.
            services.AddDistributedMemoryCache();
        }

        services.TryAddSingleton<ICacheService, CacheService>();
        services.TryAddSingleton<IEventBus, EventBus.EventBus>();

        services.AddMassTransit(configurator =>
        {
            foreach (var configureConsumer in moduleConfigureConsumers)
            {
                configureConsumer(configurator);
            }

            configurator.SetKebabCaseEndpointNameFormatter();
            configurator.UsingInMemory((context, config) =>
            {
                config.ConfigureEndpoints(context);
            });
        });

        return services;
    }

    public static WebApplicationBuilder AddQdrantHttpClient(this WebApplicationBuilder builder, string connectionName)
    {
        var connectionString = builder.Configuration.GetConnectionString($"{connectionName}_http");
        var connectionBuilder = new DbConnectionStringBuilder { ConnectionString = connectionString };
        var endpoint = new Uri((string)connectionBuilder["endpoint"]);
        var key = (string)connectionBuilder["key"];

        builder.Services.AddKeyedScoped(GetServiceKey(connectionName), (services, _) =>
        {
            var httpClient = services.GetRequiredService<HttpClient>();
            httpClient.BaseAddress = endpoint;
            httpClient.DefaultRequestHeaders.Add("api-key", key);
            return httpClient;
        });

        return builder;
    }

    public static HttpClient GetQdrantHttpClient(this IServiceProvider services, string connectionName) => 
        services.GetRequiredKeyedService<HttpClient>(GetServiceKey(connectionName));

    private static string GetServiceKey(string connectionName) => $"{connectionName}_httpclient";
}