using ECommerce.Common.Domain;
using ECommerce.Common.Domain.Auditing;

namespace ECommerce.Modules.Ticketing.Domain.Messages;

[Auditable]
public sealed class Message : Entity
{
    public Guid Id { get; private init; }

    public Guid TicketId { get; private set; }

    public bool IsCustomerMessage { get; private set; }

    public string Text { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    private Message()
    {
    }

    public static Message Create(Guid ticketId, string text, bool isCustomerMessage)
    {
        return new Message
        {
            Id = Guid.CreateVersion7(),
            TicketId = ticketId,
            IsCustomerMessage = isCustomerMessage,
            Text = text,
            CreatedAtUtc = DateTime.UtcNow,
        };
    }
}
