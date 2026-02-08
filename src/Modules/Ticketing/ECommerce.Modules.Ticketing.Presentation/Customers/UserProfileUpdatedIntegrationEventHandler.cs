using ECommerce.Common.Application.EventBus;
using ECommerce.Common.Application.Exceptions;
using ECommerce.Common.Application.Messaging;
using ECommerce.Modules.Ticketing.Application.Customers.ChangeCustomerName;
using ECommerce.Modules.Users.IntegrationEvents;

namespace ECommerce.Modules.Ticketing.Presentation.Customers;

internal sealed class UserProfileUpdatedIntegrationEventHandler(ICommandHandler<ChangeCustomerNameCommand> handler)
     : IntegrationEventHandler<UserNameChangedIntegrationEvent>
{
    public override async Task Handle(
        UserNameChangedIntegrationEvent integrationEvent, 
        CancellationToken cancellationToken = default)
    {
        var result = await handler.Handle(
            new ChangeCustomerNameCommand(
                integrationEvent.UserId,
                integrationEvent.FirstName,
                integrationEvent.LastName),
            cancellationToken);

        if (result.IsFailure)
        {
            throw new ECommerceException(nameof(ChangeCustomerNameCommand), result.Error);
        }
    }
}
