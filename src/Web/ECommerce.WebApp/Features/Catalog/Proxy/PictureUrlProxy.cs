namespace ECommerce.WebApp.Features.Catalog.Proxy;

internal static class PictureUrlProxy
{
    internal static IEndpointRouteBuilder RegisterPictureEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/catalog/items/{id:int}/picture", async (
            int id,
            IHttpClientFactory httpClientFactory) =>
        {
            var client = httpClientFactory.CreateClient("ECommerceApi");
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
