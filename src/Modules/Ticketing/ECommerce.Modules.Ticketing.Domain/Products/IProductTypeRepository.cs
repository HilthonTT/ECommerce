namespace ECommerce.Modules.Ticketing.Domain.Products;

public interface IProductTypeRepository
{
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ProductType?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    void Insert(ProductType productType);
}
