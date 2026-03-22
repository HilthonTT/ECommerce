using ECommerce.Common.Application.EventBus;

namespace ECommerce.Modules.Catalog.IntegrationEvents;

public sealed class CatalogItemStockAddedIntegrationEvent : IntegrationEvent
{
    public int CatalogItemId { get; init; }
    public int QuantityAdded { get; init; }
    public int AvailableStock { get; init; }

    public CatalogItemStockAddedIntegrationEvent(
        Guid id,
        DateTime occurredAtUtc,
        int catalogItemId,
        int quantityAdded,
        int availableStock)
        : base(id, occurredAtUtc)
    {
        CatalogItemId = catalogItemId;
        QuantityAdded = quantityAdded;
        AvailableStock = availableStock;
    }
}
