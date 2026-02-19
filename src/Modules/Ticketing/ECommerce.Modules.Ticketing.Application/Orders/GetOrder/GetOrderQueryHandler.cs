using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Domain;
using ECommerce.Modules.Ticketing.Domain.Customers;
using ECommerce.Modules.Ticketing.Domain.Orders;

namespace ECommerce.Modules.Ticketing.Application.Orders.GetOrder;

internal sealed class GetOrderQueryHandler(IOrderRepository orderRepository) : IQueryHandler<GetOrderQuery, OrderDto>
{
    public async Task<Result<OrderDto>> Handle(GetOrderQuery query, CancellationToken cancellationToken)
    {
        Order? order = await orderRepository.GetAsync(query.OrderId, cancellationToken);
        if (order is null)
        {
            return OrderErrors.NotFound(query.OrderId);
        }

        if (order.CustomerId != query.CustomerId)
        {
            return CustomerErrors.Unauthorized;
        }

        return order.ToDto();
    }
}
