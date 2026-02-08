using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Domain;
using ECommerce.Modules.Ticketing.Domain.Carts;
using ECommerce.Modules.Ticketing.Domain.Customers;
using ECommerce.Modules.Ticketing.Domain.Products;

namespace ECommerce.Modules.Ticketing.Application.Carts.RemoveItemFromCart;

internal sealed class RemoveItemFromCartCommandHandler(
    ICustomerRepository customerRepository,
    IProductRepository productRepository,
    ICartService cartService) : ICommandHandler<RemoveItemFromCartCommand>
{
    public async Task<Result> Handle(RemoveItemFromCartCommand command, CancellationToken cancellationToken)
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

        await cartService.RemoveItemAsync(customer.Id, product.Id, cancellationToken);

        return Result.Success();
    }
}
