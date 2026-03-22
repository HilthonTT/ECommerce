using ECommerce.Common.Application.EventBus;
using ECommerce.Common.Application.Messaging;
using ECommerce.Modules.Catalog.Domain.Catalog;
using ECommerce.Modules.Catalog.IntegrationEvents;

namespace ECommerce.Modules.Catalog.Application.Catalog.CreateItem;

internal sealed class ItemCreatedDomainEventHandler(IEventBus eventBus)
    : DomainEventHandler<CatalogItemCreatedDomainEvent>
{
    public override async Task Handle(
        CatalogItemCreatedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        await eventBus.PublishAsync(
            new CatalogItemCreatedIntegrationEvent(
                domainEvent.Id,
                domainEvent.OccurredAtUtc,
                domainEvent.CatalogItemId,
                domainEvent.Name,
                domainEvent.Price,
                domainEvent.CatalogTypeId,
                domainEvent.CatalogBrandId,
                domainEvent.AvailableStock,
                domainEvent.RestockThreshold,
                domainEvent.MaxStockThreshold),
            cancellationToken);
    }
}