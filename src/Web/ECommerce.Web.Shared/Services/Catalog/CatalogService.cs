using ECommerce.Web.Shared.DTOs.Catalog;
using ECommerce.Web.Shared.DTOs.Common;
using ECommerce.Web.Shared.Services.Catalog.Interfaces;
using ECommerce.Web.Shared.Services.Common;
using System.Net.Http.Json;

namespace ECommerce.Web.Shared.Services.Catalog;

internal sealed class CatalogService(IHttpClientFactory httpClientFactory) : ICatalogService
{
    private readonly HttpClient _client = httpClientFactory.CreateClient(HttpClientFactoryNames.Default);

    public async Task<CollectionResponse<CatalogBrandDto>> GetBrandsAsync(CancellationToken cancellationToken = default)
    {
        var response = await _client.GetFromJsonAsync<CollectionResponse<CatalogBrandDto>>(
            $"api/v1/catalog/brands", cancellationToken) ?? new CollectionResponse<CatalogBrandDto> { Items = [] };

        return response;
    }

    public Task<CatalogItemDto?> GetCatalogItem(int id, CancellationToken cancellationToken = default)
    {
        return _client.GetFromJsonAsync<CatalogItemDto>($"api/v1/catalog/items/{id}", cancellationToken);
    }

    public async Task<PaginationResult<CatalogItemDto>> GetPaginatedItemsAsync(
        int page, 
        int pageSize, 
        string? searchTerm = null, 
        string? sort = null, 
        Guid? catalogTypeId = null, 
        Guid? catalogBrandId = null,
        CancellationToken cancellationToken = default)
    {
        var query = new Dictionary<string, string?>
        {
            ["page"] = page.ToString(),
            ["pageSize"] = pageSize.ToString(),
            ["q"] = searchTerm,
            ["sort"] = sort,
            ["catalogTypeId"] = catalogTypeId?.ToString(),
            ["catalogBrandId"] = catalogBrandId?.ToString(),
        };

        string queryString = string.Join("&", query
            .Where(kv => kv.Value is not null)
            .Select(kv => $"{kv.Key}={Uri.EscapeDataString(kv.Value!)}"));

        string url = $"/api/v1/catalog/items?{queryString}";

        var result = await _client.GetFromJsonAsync<PaginationResult<CatalogItemDto>>(url, cancellationToken);

        return result ?? new PaginationResult<CatalogItemDto>();
    }

    public string GetPictureUrl(int id)
    {
        return $"/api/v1/catalog/items/{id}/picture";
    }

    public async Task<CollectionResponse<CatalogItemTypeDto>> GetTypesAsync(CancellationToken cancellationToken = default)
    {
        var response = await _client.GetFromJsonAsync<CollectionResponse<CatalogItemTypeDto>>(
            $"api/v1/catalog/types",
            cancellationToken) ?? new CollectionResponse<CatalogItemTypeDto> { Items = [] };

        return response;
    }
}
