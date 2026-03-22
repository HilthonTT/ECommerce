using ECommerce.Common.Application.EventBus;

namespace ECommerce.Modules.Catalog.IntegrationEvents;

public sealed class CatalogItemUpdatedIntegrationEvent : IntegrationEvent
{
    public int CatalogItemId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public decimal Price { get; init; }

    public CatalogItemUpdatedIntegrationEvent(
        Guid id,
        DateTime occurredAtUtc,
        int catalogItemId,
        string name,
        string? description,
        decimal price)
        : base(id, occurredAtUtc)
    {
        CatalogItemId = catalogItemId;
        Name = name;
        Description = description;
        Price = price;
    }
}