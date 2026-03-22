using ECommerce.Modules.Ticketing.Domain.Products;
using ECommerce.Modules.Ticketing.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Modules.Ticketing.Infrastructure.Products;

internal sealed class ProductBrandRepository(TicketingDbContext dbContext) : IProductBrandRepository
{
    public Task<ProductBrand?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return dbContext.ProductBrands.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return dbContext.ProductBrands.AnyAsync(p => p.Id == id, cancellationToken);
    }

    public void Insert(ProductBrand productBrand)
    {
        dbContext.ProductBrands.Add(productBrand);
    }
}
