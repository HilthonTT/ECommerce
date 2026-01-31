using ECommerce.Common.Domain;

namespace ECommerce.Modules.Ticketing.Domain.Products;

public sealed class ProductStockRemovedDomainEvent(Guid productId, int quantity, int newAvailableStock) : DomainEvent
{
    public Guid ProductId { get; } = productId;
    public int Quantity { get; } = quantity;
    public int NewAvailableStock { get; } = newAvailableStock;
}
