using ECommerce.Common.Domain;

namespace ECommerce.Modules.Ticketing.Domain.Products;

public sealed class ProductRestockedDomainEvent(Guid productId, int newAvailableStock) : DomainEvent
{
    public Guid ProductId { get; } = productId;
    public int NewAvailableStock { get; } = newAvailableStock;
}