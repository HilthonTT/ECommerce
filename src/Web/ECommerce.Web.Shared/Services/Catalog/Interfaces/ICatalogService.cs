using ECommerce.Web.Shared.DTOs.Catalog;
using ECommerce.Web.Shared.DTOs.Common;

namespace ECommerce.Web.Shared.Services.Catalog.Interfaces;

public interface ICatalogService
{
    Task<CatalogItemDto?> GetCatalogItem(int id, CancellationToken cancellationToken = default);

    Task<PaginationResult<CatalogItemDto>> GetPaginatedItemsAsync(
        int page,
        int pageSize,
        string? searchTerm = null,
        string? sort = null,
        Guid? catalogTypeId = null,
        Guid? catalogBrandId = null,
        CancellationToken cancellationToken = default);

    Task<CollectionResponse<CatalogBrandDto>> GetBrandsAsync(CancellationToken cancellationToken = default);

    Task<CollectionResponse<CatalogItemTypeDto>> GetTypesAsync(CancellationToken cancellationToken = default);

    string GetPictureUrl(int id);
}
