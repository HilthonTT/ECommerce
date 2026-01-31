using ECommerce.Common.Domain;

namespace ECommerce.Modules.Ticketing.Domain.Products;

public sealed class ProductRestockThresholdReachedDomainEvent(
    Guid productId,
    int currentStock,
    int restockThreshold) : DomainEvent
{
    public Guid ProductId { get; } = productId;
    public int CurrentStock { get; } = currentStock;
    public int RestockThreshold { get; } = restockThreshold;
}
