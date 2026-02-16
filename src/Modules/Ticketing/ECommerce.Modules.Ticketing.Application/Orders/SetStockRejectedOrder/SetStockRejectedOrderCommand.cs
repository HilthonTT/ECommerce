using ECommerce.Common.Application.Messaging;

namespace ECommerce.Modules.Ticketing.Application.Orders.SetStockRejectedOrder;

public sealed record SetStockRejectedOrderCommand(Guid OrderId, List<Guid> OrderStockItems) : ICommand;
