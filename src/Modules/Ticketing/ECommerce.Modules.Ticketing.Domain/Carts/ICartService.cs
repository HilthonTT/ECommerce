namespace ECommerce.Modules.Ticketing.Domain.Carts;

public interface ICartService
{
    Task<Cart> GetAsync(Guid customerId, CancellationToken cancellationToken = default);

    Task ClearAsync(Guid customerId, CancellationToken cancellationToken = default);

    Task AddItemAsync(Guid customerId, CartItem cartItem, CancellationToken cancellationToken = default);

    Task RemoveItemAsync(Guid customerId, Guid ticketTypeId, CancellationToken cancellationToken = default);
}
