using ECommerce.Common.Domain;

namespace ECommerce.Modules.Ticketing.Domain.Carts;

public static class CartErrors
{
    public static Error NotFound(Guid customerId) => Error.NotFound(
        "Cart.NotFound",
        $"The cart for customer with identifier '{customerId}' was not found.");

    public static Error ItemNotFound(int productId) => Error.NotFound(
        "Cart.ItemNotFound",
        $"The cart item with product identifier '{productId}' was not found.");
}
