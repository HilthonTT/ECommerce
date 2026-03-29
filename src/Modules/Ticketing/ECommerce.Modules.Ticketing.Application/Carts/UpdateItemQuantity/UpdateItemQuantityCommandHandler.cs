using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Domain;
using ECommerce.Modules.Ticketing.Domain.Carts;
using ECommerce.Modules.Ticketing.Domain.Customers;

namespace ECommerce.Modules.Ticketing.Application.Carts.UpdateItemQuantity;

internal sealed class UpdateItemQuantityCommandHandler(
    ICartService cartService,
    ICustomerRepository customerRepository) : ICommandHandler<UpdateItemQuantityCommand>
{
    public async Task<Result> Handle(UpdateItemQuantityCommand command, CancellationToken cancellationToken)
    {
        Customer? customer = await customerRepository.GetAsync(command.CustomerId, cancellationToken);
        if (customer is null)
        {
            return CustomerErrors.NotFound(command.CustomerId);
        }

        await cartService.UpdateQuantityAsync(customer.Id, command.ProductId, command.Quantity, cancellationToken);

        return Result.Success();
    }
}
