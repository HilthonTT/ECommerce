using ECommerce.Common.Application.Messaging;

namespace ECommerce.Modules.Ticketing.Application.Carts.UpdateItemQuantity;

public sealed record UpdateItemQuantityCommand(Guid CustomerId, int ProductId, int Quantity) : ICommand;
