using ECommerce.Common.Application.EventBus;
using ECommerce.Common.Application.Messaging;
using ECommerce.Modules.Catalog.IntegrationEvents;
using ECommerce.Modules.Ticketing.Application.Products.CreateProduct;

namespace ECommerce.Modules.Ticketing.Presentation.Products;

internal sealed class CatalogItemCreatedIntegrationEventHandler(
    ICommandHandler<CreateProductCommand> handler)
    : IntegrationEventHandler<CatalogItemCreatedIntegrationEvent>
{
    public override async Task Handle(
        CatalogItemCreatedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
    {
        await handler.Handle(
            new CreateProductCommand(
                integrationEvent.CatalogItemId,
                integrationEvent.Name,
                integrationEvent.Price,
                integrationEvent.CatalogBrandId,
                integrationEvent.CatalogTypeId,
                Description: null,
                integrationEvent.AvailableStock,
                integrationEvent.RestockThreshold,
                integrationEvent.MaxStockThreshold),
            cancellationToken);
    }
}
