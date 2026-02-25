using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Domain;
using ECommerce.Modules.Catalog.Application.Abstractions.AI;
using ECommerce.Modules.Catalog.Application.Abstractions.Data;
using ECommerce.Modules.Catalog.Domain.Catalog;
using Pgvector;

namespace ECommerce.Modules.Catalog.Application.Catalog.CreateItem;

internal sealed class CreateItemCommandHandler(
    ICatalogItemRepository catalogItemRepository, 
    ICatalogAI catalogAI,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateItemCommand, CatalogItemResponse>
{
    public async Task<Result<CatalogItemResponse>> Handle(
        CreateItemCommand command, 
        CancellationToken cancellationToken)
    {
        var catalogItem = CatalogItem.Create(
            command.CatalogItemId,
            command.Name,
            command.Description,
            command.Price,
            command.CatalogTypeId,
            command.CatalogBrandId,
            command.AvailableStock,
            command.RestockThreshold,
            command.MaxStockThreshold,
            command.PictureFileName);

        Vector? embedding = await catalogAI.GetEmbeddingAsync(catalogItem, cancellationToken);
        catalogItem.SetEmbedding(embedding);

        catalogItemRepository.Insert(catalogItem);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return catalogItem.ToResponse();
    }
}
