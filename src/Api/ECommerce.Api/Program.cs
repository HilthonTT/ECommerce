using ECommerce.Api;
using ECommerce.Api.Extensions;
using ECommerce.Api.Options;
using ECommerce.Common.Presentation.Endpoints;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddModuleConfiguration([
    "catalog",
    "ticketing",
    "webhooks",
    "users"
]);

string databaseConnectionString = builder.Configuration.GetConnectionStringOrThrow("Database");
string cacheConnectionString = builder.Configuration.GetConnectionStringOrThrow("Cache");

builder
    .AddErrorHandling()
    .AddCorsPolicy()
    .AddLogging()
    .AddObservability()
    .AddRateLimiting();

var keycloakHealthUrl = builder.Configuration.GetValue<string>("KeyCloak:HealthUrl")!;

builder.Services
    .AddHealthChecks()
    .AddNpgSql(databaseConnectionString)
    .AddRedis(cacheConnectionString)
    .AddUrlGroup(new Uri(keycloakHealthUrl), HttpMethod.Get, "Keycloak");

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();

    await app.ApplyMigrationsAsync();
}

app.UseHttpsRedirection();

app.MapEndpoints();

app.MapHealthChecks("health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseSerilogRequestLogging();

app.UseExceptionHandler();

app.UseCors(CorsOptions.PolicyName);

app.UseAuthentication();

app.UseAuthorization();

app.UseRateLimiter();

app.UseUserContextEnrichment();

await app.RunAsync();
