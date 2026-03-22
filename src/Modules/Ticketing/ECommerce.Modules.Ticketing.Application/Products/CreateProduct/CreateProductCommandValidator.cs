using FluentValidation;

namespace ECommerce.Modules.Ticketing.Application.Products.CreateProduct;

internal sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0);

        RuleFor(x => x.ProductBrandId)
            .NotEmpty();

        RuleFor(x => x.ProductTypeId)
            .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .When(x => x.Description is not null);

        RuleFor(x => x.Price)
            .GreaterThan(0);

        RuleFor(x => x.AvailableStock)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.RestockThreshold)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.MaxStockThreshold)
            .GreaterThan(0)
            .GreaterThanOrEqualTo(x => x.RestockThreshold)
            .WithMessage("MaxStockThreshold must be greater than or equal to RestockThreshold.");

        RuleFor(x => x.AvailableStock)
            .LessThanOrEqualTo(x => x.MaxStockThreshold)
            .WithMessage("AvailableStock cannot exceed MaxStockThreshold.");
    }
}
