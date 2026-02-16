using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Domain;
using ECommerce.Modules.Ticketing.Application.Abstractions.Data;
using ECommerce.Modules.Ticketing.Domain.Orders;

namespace ECommerce.Modules.Ticketing.Application.Orders.SetStockRejectedOrder;

internal sealed class SetStockRejectedOrderCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork) 
    : ICommandHandler<SetStockRejectedOrderCommand>
{
    public async Task<Result> Handle(SetStockRejectedOrderCommand command, CancellationToken cancellationToken)
    {
        Order? order = await orderRepository.GetAsync(command.OrderId, cancellationToken);
        if (order is null)
        {
            return OrderErrors.NotFound(command.OrderId);
        }

        order.SetCancelledStatusWhenStockIsRejected(command.OrderStockItems);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
