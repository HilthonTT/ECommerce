namespace ECommerce.Modules.Catalog.Domain.Catalog;

public interface ICatalogItemRepository
{
    Task<CatalogItem?> GetAsync(int id, CancellationToken cancellationToken = default);

    void Insert(CatalogItem catalogItem);

    void Remove(CatalogItem catalogItem);
}
