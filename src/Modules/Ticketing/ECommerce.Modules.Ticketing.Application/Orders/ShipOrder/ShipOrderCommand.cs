using ECommerce.Common.Application.Messaging;

namespace ECommerce.Modules.Ticketing.Application.Orders.ShipOrder;

public sealed record ShipOrderCommand(Guid OrderId, Guid CustomerId) : ICommand;
