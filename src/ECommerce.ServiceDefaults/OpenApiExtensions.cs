using Asp.Versioning;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scalar.AspNetCore;

namespace ECommerce.ServiceDefaults;

public static class OpenApiExtensions
{
    public static IApplicationBuilder UseDefaultOpenApi(this WebApplication app)
    {
        IConfiguration configuration = app.Configuration;
        IConfigurationSection openApiSection = configuration.GetSection("OpenApi");

        if (!openApiSection.Exists())
        {
            return app;
        }

        app.MapOpenApi();

        if (app.Environment.IsDevelopment())
        {
            app.MapScalarApiReference(options =>
            {
                // Disable default fonts to avoid download unnecessary fonts
                options.DefaultFonts = false;
            });

            app.MapGet("/", () => Results.Redirect("/scalar/v1")).ExcludeFromDescription();
        }

        return app;
    }

    public static IHostApplicationBuilder AddDefaultOpenApi(
        this IHostApplicationBuilder builder,
        IApiVersioningBuilder? apiVersioningBuilder = null)
    {
        IConfigurationSection openApi = builder.Configuration.GetSection("OpenApi");
        IConfigurationSection identitySection = builder.Configuration.GetSection("Identity");

        Dictionary<string, string?> scopes = identitySection.Exists()
            ? identitySection.GetRequiredSection("Scopes").GetChildren().ToDictionary(p => p.Key, p => p.Value)
            : [];

        if (!openApi.Exists())
        {
            return builder;
        }

        if (apiVersioningBuilder is not null)
        {
            IApiVersioningBuilder versioned = apiVersioningBuilder
                .AddApiExplorer(options => options.GroupNameFormat = "'v'VVV");

            string[] versions = ["v1", "v2"];

            foreach (string description in versions)
            {
                builder.Services.AddOpenApi(description, options =>
                {
                    options.ApplyApiVersionInfo(
                        openApi.GetRequiredValue("Document:Title"),
                        openApi.GetRequiredValue("Document:Description"));

                    options.ApplyAuthorizationChecks([.. scopes.Keys]);
                    options.ApplySecuritySchemeDefinitions();
                    options.ApplyOperationDeprecatedStatus();
                    options.ApplyApiVersionDescription();
                });
            }
        }

        return builder;
    }
}
