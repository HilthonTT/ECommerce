using FluentValidation;

namespace ECommerce.Modules.Ticketing.Application.Orders.SetStockConfirmedOrderStatus;

internal sealed class SetStockConfirmedOrderCommandValidator : AbstractValidator<SetStockConfirmedOrderCommand>
{
    public SetStockConfirmedOrderCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithMessage("Order ID is required.");

        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("Customer ID is required.");
    }
}
