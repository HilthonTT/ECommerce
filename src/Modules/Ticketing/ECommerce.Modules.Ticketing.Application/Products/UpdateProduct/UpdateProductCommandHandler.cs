using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Domain;
using ECommerce.Modules.Ticketing.Application.Abstractions.AI;
using ECommerce.Modules.Ticketing.Application.Abstractions.Data;
using ECommerce.Modules.Ticketing.Domain.Products;

namespace ECommerce.Modules.Ticketing.Application.Products.UpdateProduct;

internal sealed class UpdateProductCommandHandler(
    IProductRepository productRepository,
    IEmbeddingService embeddingService,
    IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateProductCommand>
{
    public async Task<Result> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
    {
        Product? product = await productRepository.GetAsync(command.ProductId, cancellationToken);

        if (product is null)
        {
            return ProductErrors.NotFound(command.ProductId);
        }

        var nameEmbedding = await embeddingService.GenerateEmbeddingAsync(
            command.Name,
            cancellationToken);

        Result result = product.UpdateDetails(
            command.Name,
            command.Description,
            command.Price,
            nameEmbedding);

        if (result.IsFailure)
        {
            return result;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}