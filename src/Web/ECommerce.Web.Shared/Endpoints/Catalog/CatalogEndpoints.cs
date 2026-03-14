using ECommerce.Web.Shared.Services.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ECommerce.Web.Shared.Endpoints.Catalog;

public static class CatalogEndpoints
{
    public static IEndpointRouteBuilder RegisterCatalogEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/catalog/items/{id:int}/picture", async (
            int id,
            IHttpClientFactory httpClientFactory) =>
        {
            var client = httpClientFactory.CreateClient(HttpClientFactoryNames.Default);
            var response = await client.GetAsync($"api/v1/catalog/items/{id}/picture");

            if (!response.IsSuccessStatusCode)
            {
                return Results.NotFound();
            }

            var contentType = response.Content.Headers.ContentType?.ToString() ?? "image/jpeg";
            var stream = await response.Content.ReadAsStreamAsync();

            return Results.Stream(stream, contentType);
        });

        return app;
    }
}
