using FluentValidation;

namespace ECommerce.Modules.Ticketing.Application.Products.CreateProductBrand;

internal sealed class CreateProductBrandCommandValidator : AbstractValidator<CreateProductBrandCommand>
{
    public CreateProductBrandCommandValidator()
    {
        RuleFor(x => x.ProductBrandId).NotEmpty();

        RuleFor(x => x.Brand).NotEmpty();
    }
}
