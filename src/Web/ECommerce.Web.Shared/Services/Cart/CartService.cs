using ECommerce.Web.Shared.DTOs.Carts;
using ECommerce.Web.Shared.Services.Authentication;
using ECommerce.Web.Shared.Services.Cart.Interfaces;

namespace ECommerce.Web.Shared.Services.Cart;

internal sealed class CartService(AuthenticatedHttpClient client) : ICartService
{
    public async Task AddToCartAsync(
        AddToCartRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var response = await client.PutAsJsonAsync("/api/v1/carts/add", request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task ClearCartAsync(CancellationToken cancellationToken = default)
    {
        var response = await client.DeleteAsync("/api/v1/carts/clear", cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task<CartDto> GetCartAsync(CancellationToken cancellationToken = default)
    {
        return await client.GetFromJsonAsync<CartDto>("api/v1/carts", cancellationToken)
            ?? new CartDto { CustomerId = Guid.CreateVersion7(), Items = [] };
    }

    public async Task RemoveAsync(int productId, CancellationToken cancellationToken = default)
    {
        var response = await client.PutAsJsonAsync("/api/v1/carts/remove", new { productId }, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateQuantityAsync(
        int productId,
        int quantity,
        CancellationToken cancellationToken = default)
    {
        var response = await client.PutAsJsonAsync(
            "/api/v1/carts/update-quantity",
            new { productId, quantity },
            cancellationToken);

        response.EnsureSuccessStatusCode();
    }
}
