using ECommerce.Common.Application.EventBus;
using ECommerce.Common.Application.Messaging;
using ECommerce.Modules.Catalog.Domain.Catalog;
using ECommerce.Modules.Catalog.IntegrationEvents;

namespace ECommerce.Modules.Catalog.Application.Catalog.UpdateItem;

internal sealed class ItemStockAddedDomainEventHandler(IEventBus eventBus)
    : DomainEventHandler<CatalogItemStockAddedDomainEvent>
{
    public override async Task Handle(
        CatalogItemStockAddedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        await eventBus.PublishAsync(
            new CatalogItemStockAddedIntegrationEvent(
                domainEvent.Id,
                domainEvent.OccurredAtUtc,
                domainEvent.CatalogItemId,
                domainEvent.QuantityAdded,
                domainEvent.NewStock),
            cancellationToken);
    }
}
