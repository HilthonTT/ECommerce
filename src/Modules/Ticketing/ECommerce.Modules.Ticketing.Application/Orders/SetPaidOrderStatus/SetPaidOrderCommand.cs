using ECommerce.Common.Application.Messaging;

namespace ECommerce.Modules.Ticketing.Application.Orders.SetPaidOrderStatus;

public sealed record SetStockConfirmedOrderCommand(Guid OrderId, Guid CustomerId) : ICommand;
