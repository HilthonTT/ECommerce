using ECommerce.Common.Application.Messaging;

namespace ECommerce.Modules.Ticketing.Application.Carts.AddItemToCart;

public sealed record AddItemToCartCommand(Guid CustomerId, Guid ProductId, int Quantity) : ICommand;