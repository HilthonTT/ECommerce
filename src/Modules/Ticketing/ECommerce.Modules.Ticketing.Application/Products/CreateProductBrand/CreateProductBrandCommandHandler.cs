using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Domain;
using ECommerce.Modules.Ticketing.Application.Abstractions.Data;
using ECommerce.Modules.Ticketing.Domain.Products;

namespace ECommerce.Modules.Ticketing.Application.Products.CreateProductBrand;

internal sealed class CreateProductBrandCommandHandler(
    IProductBrandRepository productBrandRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateProductBrandCommand>
{
    public async Task<Result> Handle(CreateProductBrandCommand command, CancellationToken cancellationToken)
    {
        ProductBrand productBrand = ProductBrand.Create(command.ProductBrandId, command.Brand);

        productBrandRepository.Insert(productBrand);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
