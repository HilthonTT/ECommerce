using ECommerce.Common.Application.Messaging;
using ECommerce.Modules.Ticketing.Application.Carts.GetCart;

namespace ECommerce.Modules.Ticketing.Application.Orders.CreateOrderDraft;

public sealed record CreateOrderDraftCommand(Guid CustomerId, List<CartItemDto> Items) : ICommand<OrderDraftDto>;
