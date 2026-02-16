using ECommerce.Modules.Ticketing.Application.Carts.GetCart;
using FluentValidation;

namespace ECommerce.Modules.Ticketing.Application.Orders.CreateOrderDraft;

internal sealed class CreateOrderDraftCommandValidator : AbstractValidator<CreateOrderDraftCommand>
{
    public CreateOrderDraftCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("Customer ID is required.");

        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("Order draft must contain at least one item.")
            .Must(items => items.Count <= 100)
            .WithMessage("Order draft cannot contain more than 100 different items.");

        RuleForEach(x => x.Items)
            .SetValidator(new CartItemDtoValidator());

        // Business rule validations
        RuleFor(x => x.Items)
            .Must(HaveUniqueProducts)
            .WithMessage("Duplicate products found in cart. Each product should appear only once.");

        RuleFor(x => x.Items)
            .Must(items => items.Sum(i => i.Quantity) <= 1000)
            .WithMessage("Total quantity in order draft cannot exceed 1000.");

        RuleFor(x => x.Items)
            .Must(HaveConsistentCurrency)
            .WithMessage("All items must have the same currency.");

        RuleFor(x => x.Items)
            .Must(HaveValidTotalPrice)
            .WithMessage("Order draft total must be greater than 0.");
    }

    private static bool HaveUniqueProducts(List<CartItemDto> items)
    {
        return items.Select(i => i.ProductId).Distinct().Count() == items.Count;
    }

    private static bool HaveConsistentCurrency(List<CartItemDto> items)
    {
        if (items == null || items.Count == 0)
            return true;

        var firstCurrency = items[0].Currency;
        return items.All(i => i.Currency.Equals(firstCurrency, StringComparison.OrdinalIgnoreCase));
    }

    private static bool HaveValidTotalPrice(List<CartItemDto> items)
    {
        decimal total = items.Sum(i => i.UnitPrice * i.Quantity);
        return total > 0;
    }
}
