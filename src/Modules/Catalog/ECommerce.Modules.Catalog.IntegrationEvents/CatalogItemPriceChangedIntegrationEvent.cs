using ECommerce.Common.Application.EventBus;

namespace ECommerce.Modules.Catalog.IntegrationEvents;

public sealed class CatalogItemPriceChangedIntegrationEvent : IntegrationEvent
{
    public int CatalogItemId { get; init; }
    public decimal PreviousPrice { get; init; }
    public decimal NewPrice { get; init; }

    public CatalogItemPriceChangedIntegrationEvent(
        Guid id,
        DateTime occurredAtUtc,
        int catalogItemId,
        decimal previousPrice,
        decimal newPrice)
        : base(id, occurredAtUtc)
    {
        CatalogItemId = catalogItemId;
        PreviousPrice = previousPrice;
        NewPrice = newPrice;
    }
}