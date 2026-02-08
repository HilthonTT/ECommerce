using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Domain;
using ECommerce.Modules.Ticketing.Domain.Carts;
using ECommerce.Modules.Ticketing.Domain.Customers;
using ECommerce.Modules.Ticketing.Domain.Products;

namespace ECommerce.Modules.Ticketing.Application.Carts.AddItemToCart;

internal sealed class AddItemToCartCommandHandler(
    ICartService cartService,
    IProductRepository productRepository,
    ICustomerRepository customerRepository) : ICommandHandler<AddItemToCartCommand>
{
    public async Task<Result> Handle(AddItemToCartCommand command, CancellationToken cancellationToken)
    {
        Customer? customer = await customerRepository.GetAsync(command.CustomerId, cancellationToken);
        if (customer is null)
        {
            return CustomerErrors.NotFound(command.CustomerId);
        }

        Product? product = await productRepository.GetAsync(command.ProductId, cancellationToken);
        if (product is null)
        {
            return ProductErrors.NotFound(command.ProductId);
        }

        var cartItem = new CartItem
        {
            ProductId = product.Id,
            ProductName = product.Name,
            UnitPrice = product.Price,
            OldUnitPrice = product.Price,
            Quantity = command.Quantity,
            // TODO: Add real picture URL when available
            PictureUrl = string.Empty,
            Currency = "USD"
        };

        await cartService.AddItemAsync(command.CustomerId, cartItem, cancellationToken);

        return Result.Success();
    }
}
