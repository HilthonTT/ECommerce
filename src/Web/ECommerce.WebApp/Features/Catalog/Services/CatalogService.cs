using ECommerce.WebApp.Features.Catalog.Models;

namespace ECommerce.WebApp.Features.Catalog.Services;

internal sealed class CatalogService(HttpClient httpClient) : ICatalogService
{
    private const string RemoteServiceBaseUrl = "api/v1/catalog/";

    public Task<CatalogItem?> GetCatalogItem(int id, CancellationToken cancellationToken = default)
    {
        string uri = $"{RemoteServiceBaseUrl}items/{id}";
        return httpClient.GetFromJsonAsync<CatalogItem>(uri, cancellationToken);
    }
}
