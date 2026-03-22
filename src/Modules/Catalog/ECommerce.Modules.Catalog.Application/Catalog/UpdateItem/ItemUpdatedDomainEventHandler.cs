using ECommerce.Common.Application.EventBus;
using ECommerce.Common.Application.Messaging;
using ECommerce.Modules.Catalog.Domain.Catalog;
using ECommerce.Modules.Catalog.IntegrationEvents;

namespace ECommerce.Modules.Catalog.Application.Catalog.UpdateItem;

internal sealed class ItemUpdatedDomainEventHandler(IEventBus eventBus)
    : DomainEventHandler<CatalogItemUpdatedDomainEvent>
{
    public override async Task Handle(
        CatalogItemUpdatedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        await eventBus.PublishAsync(
            new CatalogItemUpdatedIntegrationEvent(
                domainEvent.Id,
                domainEvent.OccurredAtUtc,
                domainEvent.CatalogItemId,
                domainEvent.Name,
                domainEvent.Description,
                domainEvent.Price),
            cancellationToken);
    }
}
