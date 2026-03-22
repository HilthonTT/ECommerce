using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Domain;
using ECommerce.Modules.Ticketing.Application.Abstractions.Data;
using ECommerce.Modules.Ticketing.Domain.Products;

namespace ECommerce.Modules.Ticketing.Application.Products.RemoveStock;

internal sealed class RemoveProductStockCommandHandler(
    IProductRepository productRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<RemoveProductStockCommand>
{
    public async Task<Result> Handle(RemoveProductStockCommand command, CancellationToken cancellationToken)
    {
        Product? product = await productRepository.GetAsync(command.ProductId, cancellationToken);

        if (product is null)
        {
            return ProductErrors.NotFound(command.ProductId);
        }

        Result<int> result = product.RemoveStock(command.QuantityRemoved);

        if (result.IsFailure)
        {
            return Result.Failure(result.Error);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}