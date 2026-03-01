using ECommerce.WebApp.Features.Catalog.Models;

namespace ECommerce.WebApp.Features.Catalog.Services;

public interface ICatalogService
{
    Task<CatalogItem?> GetCatalogItem(int id, CancellationToken cancellationToken = default);
}
