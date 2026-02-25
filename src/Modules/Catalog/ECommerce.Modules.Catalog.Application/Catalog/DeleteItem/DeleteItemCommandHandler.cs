using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Domain;
using ECommerce.Modules.Catalog.Application.Abstractions.Data;
using ECommerce.Modules.Catalog.Domain.Catalog;

namespace ECommerce.Modules.Catalog.Application.Catalog.DeleteItem;

internal sealed class DeleteItemCommandHandler(ICatalogItemRepository catalogItemRepository, IUnitOfWork unitOfWork)
    : ICommandHandler<DeleteItemCommand>
{
    public async Task<Result> Handle(DeleteItemCommand command, CancellationToken cancellationToken)
    {
        CatalogItem? catalogItem = await catalogItemRepository.GetAsync(command.CatalogItemId, cancellationToken);
        if (catalogItem is null)
        {
            return CatalogItemErrors.NotFound(command.CatalogItemId);
        }

        catalogItemRepository.Remove(catalogItem);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
