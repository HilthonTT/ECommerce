using ECommerce.Common.Application.EventBus;
using ECommerce.Common.Application.Messaging;
using ECommerce.Modules.Catalog.IntegrationEvents;
using ECommerce.Modules.Ticketing.Application.Products.UpdateProduct;

namespace ECommerce.Modules.Ticketing.Presentation.Products;

internal sealed class CatalogItemUpdatedIntegrationEventHandler(
    ICommandHandler<UpdateProductCommand> handler)
    : IntegrationEventHandler<CatalogItemUpdatedIntegrationEvent>
{
    public override async Task Handle(
        CatalogItemUpdatedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
    {
        await handler.Handle(
            new UpdateProductCommand(
                integrationEvent.CatalogItemId,
                integrationEvent.Name,
                integrationEvent.Price,
                integrationEvent.Description),
            cancellationToken);
    }
}