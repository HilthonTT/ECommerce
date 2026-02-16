using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Domain;
using ECommerce.Modules.Ticketing.Application.Abstractions.Data;
using ECommerce.Modules.Ticketing.Domain.Orders;

namespace ECommerce.Modules.Ticketing.Application.Orders.SetAwaitingOrderStatus;

internal sealed class SetAwaitingOrderCommandHandler(
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<SetAwaitingOrderCommand>
{
    public async Task<Result> Handle(SetAwaitingOrderCommand command, CancellationToken cancellationToken)
    {
        Order? order = await orderRepository.GetAsync(command.OrderId, cancellationToken);
        if (order is null)
        {
            return OrderErrors.NotFound(command.OrderId);
        }

        order.SetAwaitingValidationStatus();

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
