using ECommerce.Common.Domain;

namespace ECommerce.Modules.Ticketing.Domain.Products;

public sealed class ProductPriceChangedDomainEvent(Guid productId, decimal oldPrice, decimal newPrice) : DomainEvent
{
    public Guid ProductId { get; } = productId;
    public decimal OldPrice { get; } = oldPrice;
    public decimal NewPrice { get; } = newPrice;
}
