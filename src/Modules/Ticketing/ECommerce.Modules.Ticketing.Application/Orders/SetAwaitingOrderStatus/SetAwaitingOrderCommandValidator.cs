using FluentValidation;

namespace ECommerce.Modules.Ticketing.Application.Orders.SetAwaitingOrderStatus;

internal sealed class SetAwaitingOrderCommandValidator : AbstractValidator<SetAwaitingOrderCommand>
{
    public SetAwaitingOrderCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithMessage("Order ID is required.");

        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("Customer ID is required.");
    }
}
