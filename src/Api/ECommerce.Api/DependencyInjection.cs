using Asp.Versioning;
using ECommerce.Api.Middleware;
using ECommerce.Api.Options;
using ECommerce.Common.Application;
using ECommerce.Common.Infrastructure;
using ECommerce.Common.Infrastructure.Authentication;
using ECommerce.Modules.Catalog.Application;
using ECommerce.Modules.Catalog.Infrastructure;
using ECommerce.Modules.Ticketing.Infrastructure;
using ECommerce.Modules.Users.Infrastructure;
using ECommerce.ServiceDefaults.Clients.ChatCompletion;
using ECommerce.Webhooks.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.AI;
using Microsoft.OpenApi;
using Npgsql;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using System.Threading.RateLimiting;

namespace ECommerce.Api;

internal static class DependencyInjection
{
    public static WebApplicationBuilder AddModules(
        this WebApplicationBuilder builder, 
        string databaseConnectionString, 
        string cacheConnectionString)
    {
        builder.Services
            .AddApplication(
            [
                // TODO: Fix webhook's module name
                Webhooks.Application.AssemblyReference.Assembly,
                Modules.Catalog.Application.AssemblyReference.Assembly,
                Modules.Ticketing.Application.AssemblyReference.Assembly,
                Modules.Users.Application.AssemblyReference.Assembly
            ])
            .AddCatalogApplication()
            .AddInfrastructure(
            [
                TicketingModule.ConfigureConsumers,
                UsersModule.ConfigureConsumers,
                CatalogModule.ConfigureConsumers,
                WebhooksModule.ConfigureConsumers,
            ],
            databaseConnectionString,
            cacheConnectionString);

        builder.Services
            .AddTicketingModule(builder.Configuration)
            .AddCatalogModule(builder.Configuration)
            .AddWebhooksModule(builder.Configuration)
            .AddUsersModule(builder.Configuration);

        return builder;
    }

    public static WebApplicationBuilder AddLogging(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, loggerConfig) =>
        {
            loggerConfig.ReadFrom.Configuration(context.Configuration);
        });

        return builder;
    }

    public static WebApplicationBuilder AddErrorHandling(this WebApplicationBuilder builder)
    {
        builder.Services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
            };
        });

        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

        return builder;
    }

    public static WebApplicationBuilder AddObservability(this WebApplicationBuilder builder)
    {
        builder.Services.AddOpenTelemetry()
            .ConfigureResource(resourceBuilder => resourceBuilder.AddService(builder.Environment.ApplicationName))
            .WithTracing(tracerProviderBuilder => tracerProviderBuilder
                .AddHttpClientInstrumentation()
                .AddAspNetCoreInstrumentation()
                .AddNpgsql())
            .WithMetrics(meterProviderBuilder => meterProviderBuilder
                .AddHttpClientInstrumentation()
                .AddAspNetCoreInstrumentation()
                .AddRuntimeInstrumentation());

        builder.Logging.AddOpenTelemetry(openTelemetryLoggerOptions =>
        {
            openTelemetryLoggerOptions.IncludeScopes = true;
            openTelemetryLoggerOptions.IncludeFormattedMessage = true;
        });

        return builder;
    }

    public static WebApplicationBuilder AddCorsPolicy(this WebApplicationBuilder builder)
    {
        CorsOptions corsOptions = builder.Configuration.GetSection(CorsOptions.SectionName).Get<CorsOptions>()!;

        builder.Services.AddCors(options =>
        {
            options.AddPolicy(CorsOptions.PolicyName, policy =>
            {
                policy
                    .WithOrigins(corsOptions.AllowedOrigins)
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        return builder;
    }

    public static WebApplicationBuilder AddRateLimiting(this WebApplicationBuilder builder)
    {
        builder.Services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.OnRejected = async (context, token) =>
            {
                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out TimeSpan retryAfter))
                {
                    context.HttpContext.Response.Headers.RetryAfter = $"{retryAfter.TotalSeconds}";
                    ProblemDetailsFactory problemDetailsFactory = context.HttpContext.RequestServices.GetRequiredService<ProblemDetailsFactory>();
                    ProblemDetails problemDetails = problemDetailsFactory.CreateProblemDetails(
                        context.HttpContext,
                        StatusCodes.Status429TooManyRequests, "Too Many Requests",
                        detail: $"Too many requests. Please try again after {retryAfter.TotalSeconds} seconds.");
                    await context.HttpContext.Response.WriteAsJsonAsync(problemDetails, token);
                }
            };

            options.AddPolicy("default", context =>
            {
                Guid userId = context.User.GetUserIdOrEmpty();
                if (userId == Guid.Empty)
                {
                    return RateLimitPartition.GetTokenBucketLimiter(userId.ToString(), _ =>
                       new TokenBucketRateLimiterOptions
                       {
                           TokenLimit = 100,
                           QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                           QueueLimit = 5,
                           ReplenishmentPeriod = TimeSpan.FromMinutes(1),
                           TokensPerPeriod = 25
                       });
                }

                return RateLimitPartition.GetFixedWindowLimiter("anonymous", _ =>
                    new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 5,
                        Window = TimeSpan.FromMinutes(1)
                    });
            });
        });

        return builder;
    }

    public static WebApplicationBuilder AddChatCompletionService(
        this WebApplicationBuilder builder, 
        string serviceName)
    {
        ChatClientBuilder chatClientBuilder = (builder.Configuration[$"{serviceName}:Type"] == "ollama") ?
            builder.AddOllamaChatClient(serviceName) :
            builder.AddOpenAIChatClient(serviceName);

        chatClientBuilder
            .UseFunctionInvocation()
            .UseCachingForTest()
            .UseOpenTelemetry(configure: c => c.EnableSensitiveData = true);

        return builder;
    }

    public static WebApplicationBuilder AddApiDocumentation(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "ECommerce API", Version = "v1" });
            options.CustomSchemaIds(t => t.FullName?.Replace('+', '.'));
        });
        return builder;
    }

    public static WebApplicationBuilder AddApiVersioning(this WebApplicationBuilder builder)
    {
        builder.Services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1);
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
        }).AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'V";
            options.SubstituteApiVersionInUrl = true;
        });

        return builder;
    }
}
