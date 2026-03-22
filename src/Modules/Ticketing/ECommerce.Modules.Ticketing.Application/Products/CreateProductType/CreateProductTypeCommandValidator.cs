using FluentValidation;

namespace ECommerce.Modules.Ticketing.Application.Products.CreateProductType;

internal sealed class CreateProductTypeCommandValidator : AbstractValidator<CreateProductTypeCommand>
{
    public CreateProductTypeCommandValidator()
    {
        RuleFor(x => x.ProductTypeId).NotEmpty();

        RuleFor(x => x.Type).NotEmpty();
    }
}
