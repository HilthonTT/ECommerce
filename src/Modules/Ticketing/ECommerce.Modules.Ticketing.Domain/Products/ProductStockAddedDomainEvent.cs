using ECommerce.Common.Domain;

namespace ECommerce.Modules.Ticketing.Domain.Products;

public sealed class ProductStockAddedDomainEvent(int productId, int quantity, int newAvailableStock) : DomainEvent
{
    public int ProductId { get; } = productId;
    public int Quantity { get; } = quantity;
    public int NewAvailableStock { get; } = newAvailableStock;
}
