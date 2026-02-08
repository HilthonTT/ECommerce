using ECommerce.Common.Application.EventBus;
using ECommerce.Common.Application.Exceptions;
using ECommerce.Common.Application.Messaging;
using ECommerce.Modules.Ticketing.Application.Customers.CreateCustomer;
using ECommerce.Modules.Users.IntegrationEvents;

namespace ECommerce.Modules.Ticketing.Presentation.Customers;

internal sealed class UserRegisteredIntegrationEventHandler(ICommandHandler<CreateCustomerCommand> handler)
    : IntegrationEventHandler<UserRegisteredIntegrationEvent>
{
    public override async Task Handle(
        UserRegisteredIntegrationEvent integrationEvent, 
        CancellationToken cancellationToken = default)
    {
        var result = await handler.Handle(
            new CreateCustomerCommand(
                integrationEvent.UserId,
                integrationEvent.Email,
                integrationEvent.FirstName,
                integrationEvent.LastName),
            cancellationToken);

        if (result.IsFailure)
        {
            throw new ECommerceException(nameof(CreateCustomerCommand), result.Error);
        }
    }
}