using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Domain;
using ECommerce.Modules.Ticketing.Domain.Customers;

namespace ECommerce.Modules.Ticketing.Application.Customers.GetCustomer;

internal sealed class GetCustomerQueryHandler(ICustomerRepository customers)
    : IQueryHandler<GetCustomerQuery, Customer>
{
    public async Task<Result<Customer>> Handle(GetCustomerQuery query, CancellationToken cancellationToken)
    {
        var customer = await customers.GetAsync(query.CustomerId, cancellationToken);
        if (customer is null)
            return CustomerErrors.NotFound(query.CustomerId);

        return customer;
    }
}