using ECommerce.Common.Domain;

namespace ECommerce.Modules.Ticketing.Domain.Products;

public sealed class ProductPriceChangedDomainEvent(int productId, decimal oldPrice, decimal newPrice) : DomainEvent
{
    public int ProductId { get; } = productId;
    public decimal OldPrice { get; } = oldPrice;
    public decimal NewPrice { get; } = newPrice;
}
