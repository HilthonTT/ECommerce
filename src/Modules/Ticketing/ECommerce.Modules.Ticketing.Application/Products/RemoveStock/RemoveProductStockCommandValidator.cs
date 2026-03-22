using FluentValidation;

namespace ECommerce.Modules.Ticketing.Application.Products.RemoveStock;

internal sealed class RemoveProductStockCommandValidator : AbstractValidator<RemoveProductStockCommand>
{
    public RemoveProductStockCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0);

        RuleFor(x => x.QuantityRemoved)
            .GreaterThan(0);
    }
}