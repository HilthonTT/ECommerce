using ECommerce.Common.Application.EventBus;

namespace ECommerce.Modules.Catalog.IntegrationEvents;

public sealed class CatalogItemStockRemovedIntegrationEvent : IntegrationEvent
{
    public int CatalogItemId { get; init; }
    public int QuantityRemoved { get; init; }
    public int AvailableStock { get; init; }

    public CatalogItemStockRemovedIntegrationEvent(
        Guid id,
        DateTime occurredAtUtc,
        int catalogItemId,
        int quantityRemoved,
        int availableStock)
        : base(id, occurredAtUtc)
    {
        CatalogItemId = catalogItemId;
        QuantityRemoved = quantityRemoved;
        AvailableStock = availableStock;
    }
}