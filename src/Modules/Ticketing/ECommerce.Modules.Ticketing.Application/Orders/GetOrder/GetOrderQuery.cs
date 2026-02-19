using ECommerce.Common.Application.Messaging;

namespace ECommerce.Modules.Ticketing.Application.Orders.GetOrder;

public sealed record GetOrderQuery(Guid OrderId, Guid CustomerId) : IQuery<OrderDto>;
