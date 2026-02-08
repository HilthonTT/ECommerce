using ECommerce.Common.Application.Messaging;

namespace ECommerce.Modules.Ticketing.Application.Customers.CreateCustomer;

public sealed record CreateCustomerCommand(Guid CustomerId, string Email, string FirstName, string LastName) : ICommand;
