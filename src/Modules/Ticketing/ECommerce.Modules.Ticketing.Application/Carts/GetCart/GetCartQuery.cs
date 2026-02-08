using ECommerce.Common.Application.Messaging;
using ECommerce.Modules.Ticketing.Domain.Carts;

namespace ECommerce.Modules.Ticketing.Application.Carts.GetCart;

public sealed record GetCartQuery(Guid CustomerId) : IQuery<CartDto>;
