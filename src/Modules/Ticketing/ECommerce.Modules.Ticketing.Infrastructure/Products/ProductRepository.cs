using ECommerce.Modules.Ticketing.Domain.Products;
using ECommerce.Modules.Ticketing.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Modules.Ticketing.Infrastructure.Products;

internal sealed class ProductRepository(TicketingDbContext dbContext) : IProductRepository
{
    public Task<Product?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return dbContext.Products
            .Include(p => p.ProductBrand)
            .Include(p => p.ProductType)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public void Insert(Product product)
    {
        dbContext.Products.Add(product);
    }

    public void InsertRange(IEnumerable<Product> products)
    {
        dbContext.Products.AddRange(products);
    }

    public void Remove(Product product)
    {
        dbContext.Products.Remove(product);
    }
}
