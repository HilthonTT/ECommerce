using ECommerce.Modules.Ticketing.Domain.Tickets;

namespace ECommerce.Modules.Ticketing.Application.Tickets.GetTicket;

public sealed record TicketDetailsResult(
    Guid Id, 
    DateTime CreatedAtUtc,
    Guid CustomerId,
    string CustomerFullName,
    string? ShortSummary,
    string? LongSummary,
    int? ProductId,
    string? ProductBrand,
    string? ProductName,
    TicketType Type,
    TicketStatus Status,
    int? CustomerSatisfaction,
    List<TicketDetailsResult.TicketDetailsResultMessage> Messages)
{
    public sealed record TicketDetailsResultMessage(
        Guid MessageId, 
        DateTime CreatedAtUtc, 
        bool IsCustomerMessage, 
        string MessageText);
}
