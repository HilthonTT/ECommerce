using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Domain;
using ECommerce.Modules.Catalog.Application.Abstractions.Data;
using ECommerce.Modules.Catalog.Domain.Catalog;

namespace ECommerce.Modules.Catalog.Application.Catalog.UpdateItem;

internal sealed class UpdateItemCommandHandler(
    ICatalogItemRepository catalogItemRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateItemCommand>
{
    public async Task<Result> Handle(UpdateItemCommand command, CancellationToken cancellationToken)
    {
        CatalogItem? catalogItem = await catalogItemRepository.GetAsync(command.CatalogItemId, cancellationToken);
        if (catalogItem is null)
        {
            return CatalogItemErrors.NotFound(command.CatalogItemId);
        }

        Result result = catalogItem.UpdateDetails(command.Name, command.Description, command.Price);
        if (result.IsFailure)
        {
            return result;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
