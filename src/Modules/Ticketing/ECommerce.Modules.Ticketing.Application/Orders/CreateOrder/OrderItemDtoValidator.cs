using FluentValidation;

namespace ECommerce.Modules.Ticketing.Application.Orders.CreateOrder;

internal sealed class OrderItemDtoValidator : AbstractValidator<OrderItemDto>
{
    public OrderItemDtoValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Product ID is required.");

        RuleFor(x => x.ProductName)
            .NotEmpty()
            .WithMessage("Product name is required.")
            .MaximumLength(200)
            .WithMessage("Product name must not exceed 200 characters.");

        RuleFor(x => x.UnitPrice)
            .GreaterThan(0)
            .WithMessage("Unit price must be greater than 0.")
            .LessThanOrEqualTo(1_000_000)
            .WithMessage("Unit price cannot exceed 1,000,000.");

        RuleFor(x => x.Discount)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Discount cannot be negative.")
            .LessThanOrEqualTo(x => x.UnitPrice)
            .WithMessage("Discount cannot exceed unit price.")
            .Must((item, discount) => discount < item.UnitPrice)
            .When(x => x.UnitPrice > 0)
            .WithMessage("Discount must be less than unit price (cannot be 100% off).");

        RuleFor(x => x.Units)
            .GreaterThan(0)
            .WithMessage("Units must be at least 1.")
            .LessThanOrEqualTo(100)
            .WithMessage("Units cannot exceed 100 per item.");

        RuleFor(x => x.PictureUrl)
            .NotEmpty()
            .WithMessage("Picture URL is required.")
            .MaximumLength(2000)
            .WithMessage("Picture URL must not exceed 2000 characters.")
            .Must(BeAValidUrl)
            .WithMessage("Picture URL must be a valid HTTP or HTTPS URL.");
    }

    private static bool BeAValidUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return false;
        }

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uriResult))
        {
            return false;
        }

        return uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps;
    }
}