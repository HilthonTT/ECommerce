using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Domain;
using ECommerce.Modules.Ticketing.Application.Abstractions.Data;
using ECommerce.Modules.Ticketing.Domain.Customers;

namespace ECommerce.Modules.Ticketing.Application.Customers.ChangeCustomerName;

internal sealed class ChangeCustomerNameCommandHandler(
    ICustomerRepository customerRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<ChangeCustomerNameCommand>
{
    public async Task<Result> Handle(ChangeCustomerNameCommand request, CancellationToken cancellationToken)
    {
        Customer? customer = await customerRepository.GetAsync(request.CustomerId, cancellationToken);
        if (customer is null)
        {
            return CustomerErrors.NotFound(request.CustomerId);
        }

        customer.ChangeName(request.FirstName, request.LastName);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}