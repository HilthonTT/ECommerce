using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Domain;
using ECommerce.Modules.Ticketing.Application.Abstractions.Data;
using ECommerce.Modules.Ticketing.Domain.Carts;
using ECommerce.Modules.Ticketing.Domain.Customers;
using ECommerce.Modules.Ticketing.Domain.Orders;
using ECommerce.Modules.Ticketing.Domain.Pricing;
using ECommerce.Modules.Ticketing.Domain.Products;

namespace ECommerce.Modules.Ticketing.Application.Orders.CreateOrder;

internal sealed class CreateOrderCommandHandler(
    ICustomerRepository customerRepository,
    IOrderRepository orderRepository,
    IProductRepository productRepository,
    IPricingService pricingService,
    IUnitOfWork unitOfWork,
    ICartService cartService) : ICommandHandler<CreateOrderCommand>
{
    public async Task<Result> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        Customer? customer = await customerRepository.GetAsync(command.CustomerId, cancellationToken);
        if (customer is null)
        {
            return CustomerErrors.NotFound(command.CustomerId);
        }

        var address = new Address(command.Street,
            command.City,
            command.State,
            command.Country,
            command.ZipCode);

        Result<Order> orderResult = Order.Create(customer.Id, address);
        if (orderResult.IsFailure)
        {
            return orderResult.Error;
        }
        Order order = orderResult.Value;

        var cart = await cartService.GetAsync(command.CustomerId, cancellationToken);

        foreach (CartItem item in cart.Items)
        {
            Product? product = await productRepository.GetAsync(item.ProductId, cancellationToken);
            if (product is null)
            {
                return ProductErrors.NotFound(item.ProductId);
            }

            Result<ProductPrice> priceResult = pricingService.CalculatePrice(
               product, item.Quantity, command.CouponCode);

            if (priceResult.IsFailure)
            {
                return priceResult.Error;
            }

            ProductPrice price = priceResult.Value;

            order.AddItem(
                order.Currency,
                item.ProductId,
                item.ProductName,
                price.DiscountedUnitPrice,
                price.DiscountPercentage,
                item.PictureUrl,
                item.Quantity);
        }

        orderRepository.Insert(order);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
