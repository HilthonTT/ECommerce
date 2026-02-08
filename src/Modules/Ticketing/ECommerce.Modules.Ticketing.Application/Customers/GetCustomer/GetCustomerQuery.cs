using ECommerce.Common.Application.Messaging;
using ECommerce.Modules.Ticketing.Domain.Customers;

namespace ECommerce.Modules.Ticketing.Application.Customers.GetCustomer;

public sealed record GetCustomerQuery(Guid CustomerId) : IQuery<Customer>;
