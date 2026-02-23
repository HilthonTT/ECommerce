using ECommerce.Modules.Catalog.Domain.Catalog;
using ECommerce.Modules.Catalog.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Modules.Catalog.Infrastructure.Catalog;

internal sealed class CatalogItemRepository(CatalogDbContext dbContext) : ICatalogItemRepository
{
    public Task<CatalogItem?> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        return dbContext.CatalogItems
            .Include(c => c.CatalogBrand)
            .Include(c => c.CatalogType)
            .AsSplitQuery()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public void Insert(CatalogItem catalogItem)
    {
        dbContext.CatalogItems.Add(catalogItem);
    }

    public void Remove(CatalogItem catalogItem)
    {
        dbContext.CatalogItems.Remove(catalogItem);
    }
}
