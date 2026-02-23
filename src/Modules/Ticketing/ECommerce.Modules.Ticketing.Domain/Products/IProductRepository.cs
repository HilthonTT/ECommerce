namespace ECommerce.Modules.Ticketing.Domain.Products;

public interface IProductRepository
{
    Task<Product?> GetAsync(int id, CancellationToken cancellationToken = default);

    void Insert(Product product);

    void InsertRange(IEnumerable<Product> products);

    void Remove(Product product);
}
