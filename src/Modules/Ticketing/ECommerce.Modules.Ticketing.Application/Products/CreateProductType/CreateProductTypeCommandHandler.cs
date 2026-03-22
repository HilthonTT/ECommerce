using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Domain;
using ECommerce.Modules.Ticketing.Application.Abstractions.Data;
using ECommerce.Modules.Ticketing.Domain.Products;

namespace ECommerce.Modules.Ticketing.Application.Products.CreateProductType;

internal sealed class CreateProductTypeCommandHandler(
    IProductTypeRepository productTypeRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateProductTypeCommand>
{
    public async Task<Result> Handle(CreateProductTypeCommand command, CancellationToken cancellationToken)
    {
        ProductType productType = ProductType.Create(command.ProductTypeId, command.Type);

        productTypeRepository.Insert(productType);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
