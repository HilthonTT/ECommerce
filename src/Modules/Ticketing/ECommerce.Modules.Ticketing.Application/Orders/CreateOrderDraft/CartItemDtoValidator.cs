using ECommerce.Modules.Ticketing.Application.Carts.GetCart;
using FluentValidation;

namespace ECommerce.Modules.Ticketing.Application.Orders.CreateOrderDraft;

internal sealed class CartItemDtoValidator : AbstractValidator<CartItemDto>
{
    private static readonly HashSet<string> SupportedCurrencies = new()
    {
        "USD", "EUR", "GBP", "JPY", "AUD", "CAD", "CHF", "CNY", "SEK", "NZD"
    };

    public CartItemDtoValidator()
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

        RuleFor(x => x.OldUnitPrice)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Old unit price cannot be negative.")
            .GreaterThanOrEqualTo(x => x.UnitPrice)
            .WithMessage("Old unit price must be greater than or equal to current unit price.")
            .LessThanOrEqualTo(1_000_000)
            .WithMessage("Old unit price cannot exceed 1,000,000.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be at least 1.")
            .LessThanOrEqualTo(100)
            .WithMessage("Quantity cannot exceed 100 per item.");

        RuleFor(x => x.PictureUrl)
            .NotEmpty()
            .WithMessage("Picture URL is required.")
            .MaximumLength(2000)
            .WithMessage("Picture URL must not exceed 2000 characters.")
            .Must(BeAValidUrl)
            .WithMessage("Picture URL must be a valid HTTP or HTTPS URL.");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .WithMessage("Currency is required.")
            .Length(3)
            .WithMessage("Currency must be a 3-letter ISO code (e.g., USD, EUR, GBP).")
            .Matches("^[A-Z]{3}$")
            .WithMessage("Currency must be a valid 3-letter uppercase ISO code.")
            .Must(currency => SupportedCurrencies.Contains(currency))
            .WithMessage($"Currency must be one of: {string.Join(", ", SupportedCurrencies)}.");
    }

    private static bool BeAValidUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return false;

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uriResult))
            return false;

        return uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps;
    }
}
