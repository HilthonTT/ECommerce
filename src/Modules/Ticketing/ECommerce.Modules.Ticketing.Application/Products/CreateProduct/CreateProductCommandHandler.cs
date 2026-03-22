using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Domain;
using ECommerce.Modules.Ticketing.Application.Abstractions.AI;
using ECommerce.Modules.Ticketing.Application.Abstractions.Data;
using ECommerce.Modules.Ticketing.Domain.Products;

namespace ECommerce.Modules.Ticketing.Application.Products.CreateProduct;

internal sealed class CreateProductCommandHandler(
    IProductRepository productRepository, 
    IProductBrandRepository productBrandRepository,
    IProductTypeRepository productTypeRepository,
    IEmbeddingService embeddingService,
    IUnitOfWork unitOfWork) 
    : ICommandHandler<CreateProductCommand>
{
    public async Task<Result> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        if (!await productBrandRepository.ExistsAsync(command.ProductBrandId, cancellationToken))
        {
            return ProductBrandErrors.NotFound(command.ProductBrandId);
        }

        if (!await productTypeRepository.ExistsAsync(command.ProductTypeId, cancellationToken))
        {
            return ProductTypeErrors.NotFound(command.ProductTypeId);
        }

        var nameEmbedding = await embeddingService.GenerateEmbeddingAsync(
            command.Name,
            cancellationToken);

        var result = Product.Create(
            command.ProductId,
            command.Name,
            command.Price,
            command.ProductBrandId,
            command.ProductTypeId,
            nameEmbedding,
            command.Description,
            command.AvailableStock,
            command.RestockThreshold,
            command.MaxStockThreshold);

        if (result.IsFailure)
        {
            return Result.Failure(result.Error);
        }

        productRepository.Insert(result.Value);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
