using ECommerce.Common.Application.Messaging;

namespace ECommerce.Modules.Ticketing.Application.Carts.ClearCart;

public sealed record ClearCartCommand(Guid CustomerId) : ICommand;
