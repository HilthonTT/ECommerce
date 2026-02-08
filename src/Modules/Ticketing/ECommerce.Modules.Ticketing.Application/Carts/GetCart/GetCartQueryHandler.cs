using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Domain;
using ECommerce.Modules.Ticketing.Domain.Carts;

namespace ECommerce.Modules.Ticketing.Application.Carts.GetCart;

internal sealed class GetCartQueryHandler(ICartService cartService) : IQueryHandler<GetCartQuery, CartDto>
{
    public async Task<Result<CartDto>> Handle(GetCartQuery query, CancellationToken cancellationToken)
    {
        Cart cart = await cartService.GetAsync(query.CustomerId, cancellationToken);

        var cartDto = new CartDto
        {
            CustomerId = cart.CustomerId,
            Items = cart.Items.Select(item => new CartItemDto
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                UnitPrice = item.UnitPrice,
                OldUnitPrice = item.OldUnitPrice,
                Quantity = item.Quantity,
                PictureUrl = item.PictureUrl,
                Currency = item.Currency
            }).ToList()
        };

        return cartDto;
    }
}
