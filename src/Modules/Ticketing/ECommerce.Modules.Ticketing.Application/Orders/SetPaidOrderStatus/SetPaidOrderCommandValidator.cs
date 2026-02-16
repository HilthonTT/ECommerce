using FluentValidation;

namespace ECommerce.Modules.Ticketing.Application.Orders.SetPaidOrderStatus;

internal sealed class SetPaidOrderCommandValidator : AbstractValidator<SetStockConfirmedOrderCommand>
{
    public SetPaidOrderCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithMessage("Order ID is required.");

        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("Customer ID is required.");
    }
}
