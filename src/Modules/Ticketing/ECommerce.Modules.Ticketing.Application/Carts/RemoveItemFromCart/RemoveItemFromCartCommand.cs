using ECommerce.Common.Application.Messaging;

namespace ECommerce.Modules.Ticketing.Application.Carts.RemoveItemFromCart;

public sealed record RemoveItemFromCartCommand(Guid CustomerId, int ProductId) : ICommand;