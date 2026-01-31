using ECommerce.Common.Application.Caching;
using ECommerce.Modules.Ticketing.Domain.Carts;

namespace ECommerce.Modules.Ticketing.Infrastructure.Carts;

internal sealed class CartService(ICacheService cacheService) : ICartService
{
    private static readonly TimeSpan DefaultExpiration = TimeSpan.FromHours(3);

    public async Task AddItemAsync(Guid customerId, CartItem cartItem, CancellationToken cancellationToken = default)
    {
        string cacheKey = CreateCacheKey(customerId);

        Cart cart = await GetAsync(customerId, cancellationToken);

        CartItem? existingCartIem = cart.Items.Find(item => item.ProductId == cartItem.ProductId);
        if (existingCartIem is null)
        {
            cart.Items.Add(cartItem);
        }
        else
        {
            existingCartIem.Quantity += cartItem.Quantity;
        }

        await cacheService.SetAsync(cacheKey, cart, DefaultExpiration, cancellationToken);
    }

    public async Task ClearAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        string cacheKey = CreateCacheKey(customerId);

        var cart = Cart.CreateDefault(customerId);

        await cacheService.SetAsync(cacheKey, cart, DefaultExpiration, cancellationToken);
    }

    public async Task<Cart> GetAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        string cacheKey = CreateCacheKey(customerId);

        var cart = await cacheService.GetAsync<Cart>(cacheKey, cancellationToken) ?? Cart.CreateDefault(customerId);

        return cart;
    }

    public Task RemoveItemAsync(Guid customerId, Guid ticketTypeId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    private static string CreateCacheKey(Guid customerId) => $"carts:{customerId}";
}
