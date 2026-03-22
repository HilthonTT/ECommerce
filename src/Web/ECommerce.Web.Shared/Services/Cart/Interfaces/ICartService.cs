using ECommerce.Web.Shared.DTOs.Carts;

namespace ECommerce.Web.Shared.Services.Cart.Interfaces;

public interface ICartService
{
    Task AddToCartAsync(AddToCartRequestDto request, CancellationToken cancellationToken = default);

    Task ClearCartAsync(CancellationToken cancellationToken = default);

    Task<CartDto> GetCartAsync(CancellationToken cancellationToken = default);

    Task RemoveAsync(int productId, CancellationToken cancellationToken = default);
}
