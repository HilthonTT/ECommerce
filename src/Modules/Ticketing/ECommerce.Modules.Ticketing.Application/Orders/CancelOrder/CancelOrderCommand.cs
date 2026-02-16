using ECommerce.Common.Application.Messaging;

namespace ECommerce.Modules.Ticketing.Application.Orders.CancelOrder;

public sealed record CancelOrderCommand(Guid OrderId, Guid CustomerId) : ICommand;
