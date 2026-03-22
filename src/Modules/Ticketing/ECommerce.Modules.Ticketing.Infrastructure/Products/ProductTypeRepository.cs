using ECommerce.Modules.Ticketing.Domain.Products;
using ECommerce.Modules.Ticketing.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Modules.Ticketing.Infrastructure.Products;

internal sealed class ProductTypeRepository(TicketingDbContext dbContext) : IProductTypeRepository
{
    public Task<ProductType?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return dbContext.ProductTypes.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return dbContext.ProductTypes.AnyAsync(p => p.Id == id, cancellationToken);
    }

    public void Insert(ProductType productType)
    {
        dbContext.ProductTypes.Add(productType);
    }
}
