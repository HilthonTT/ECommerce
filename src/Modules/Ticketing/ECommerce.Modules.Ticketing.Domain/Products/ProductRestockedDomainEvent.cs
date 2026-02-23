using ECommerce.Common.Domain;

namespace ECommerce.Modules.Ticketing.Domain.Products;

public sealed class ProductRestockedDomainEvent(int productId, int newAvailableStock) : DomainEvent
{
    public int ProductId { get; } = productId;
    public int NewAvailableStock { get; } = newAvailableStock;
}