using ECommerce.WebApp.Features.Catalog.Models;

namespace ECommerce.WebApp.Features.Catalog.Services;

internal sealed class CatalogService(IHttpClientFactory httpClientFactory) : ICatalogService
{
    private readonly HttpClient _client = httpClientFactory.CreateClient("ECommerceApi");

    public Task<CatalogItem?> GetCatalogItem(int id, CancellationToken cancellationToken = default)
    {
        return _client.GetFromJsonAsync<CatalogItem>($"api/v1/catalog/items/{id}", cancellationToken);
    }

    public string GetPictureUrl(int id)
    {
        return $"/api/v1/catalog/items/{id}/picture";
    }
}
