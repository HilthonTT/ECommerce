using ECommerce.Common.Application.Messaging;

namespace ECommerce.Modules.Ticketing.Application.Orders.SetAwaitingOrderStatus;

public sealed record SetAwaitingOrderCommand(Guid OrderId, Guid CustomerId) : ICommand;
