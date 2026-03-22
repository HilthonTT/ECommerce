namespace ECommerce.Modules.Ticketing.Domain.Products;

public interface IProductBrandRepository
{
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ProductBrand?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    void Insert(ProductBrand productBrand);
}
