using ECommerce.Common.Domain;

namespace ECommerce.Modules.Ticketing.Domain.Products;

public sealed class ProductRestockThresholdReachedDomainEvent(
    int productId,
    int currentStock,
    int restockThreshold) : DomainEvent
{
    public int ProductId { get; } = productId;
    public int CurrentStock { get; } = currentStock;
    public int RestockThreshold { get; } = restockThreshold;
}
