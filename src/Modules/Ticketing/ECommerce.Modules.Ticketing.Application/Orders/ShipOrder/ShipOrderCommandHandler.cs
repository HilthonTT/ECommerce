using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Domain;
using ECommerce.Modules.Ticketing.Application.Abstractions.Data;
using ECommerce.Modules.Ticketing.Domain.Orders;

namespace ECommerce.Modules.Ticketing.Application.Orders.ShipOrder;

internal sealed class ShipOrderCommandHandler(
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<ShipOrderCommand>
{
    public async Task<Result> Handle(ShipOrderCommand command, CancellationToken cancellationToken)
    {
        Order? order = await orderRepository.GetAsync(command.OrderId, cancellationToken);
        if (order is null)
        {
            return OrderErrors.NotFound(command.OrderId);
        }

        order.SetShippedStatus();

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
