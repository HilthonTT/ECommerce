using FluentValidation;

namespace ECommerce.Modules.Ticketing.Application.Orders.SetStockRejectedOrder;

internal sealed class SetStockRejectedOrderCommandValidator : AbstractValidator<SetStockRejectedOrderCommand>
{
    public SetStockRejectedOrderCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithMessage("Order ID is required.");

        RuleFor(x => x.OrderStockItems)
            .NotEmpty()
            .WithMessage("Order IDs are required.");
    }
}
