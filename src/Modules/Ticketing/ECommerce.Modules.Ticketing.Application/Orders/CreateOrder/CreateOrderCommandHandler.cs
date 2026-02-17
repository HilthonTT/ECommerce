using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Domain;
using ECommerce.Modules.Ticketing.Application.Abstractions.Data;
using ECommerce.Modules.Ticketing.Domain.Carts;
using ECommerce.Modules.Ticketing.Domain.Customers;
using ECommerce.Modules.Ticketing.Domain.Orders;

namespace ECommerce.Modules.Ticketing.Application.Orders.CreateOrder;

internal sealed class CreateOrderCommandHandler(
    ICustomerRepository customerRepository,
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork,
    ICartService cartService) : ICommandHandler<CreateOrderCommand>
{
    private const decimal Discount = 0; // TODO: Add actual discount logic

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
            order.AddItem(
                order.Currency, 
                item.ProductId, 
                item.ProductName,
                item.UnitPrice,
                Discount, 
                item.PictureUrl, 
                item.Quantity);
        }

        orderRepository.Insert(order);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
