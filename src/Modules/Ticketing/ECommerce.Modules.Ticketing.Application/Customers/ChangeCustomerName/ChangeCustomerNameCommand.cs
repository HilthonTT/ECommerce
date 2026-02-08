using ECommerce.Common.Application.Messaging;

namespace ECommerce.Modules.Ticketing.Application.Customers.ChangeCustomerName;

public sealed record ChangeCustomerNameCommand(Guid CustomerId, string FirstName, string LastName) : ICommand;
