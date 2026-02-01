using ECommerce.Modules.Ticketing.Domain.Tickets;
using static ECommerce.Modules.Ticketing.Application.Tickets.GetTickets.GetTicketsResponse;

namespace ECommerce.Modules.Ticketing.Application.Tickets.GetTickets;

public sealed record GetTicketsResponse(
    List<GetTicketResponseItem> Items,
    int TotalCount,
    int TotalOpenCount,
    int TotalClosedCount)
{
    public sealed class GetTicketResponseItem
    {
        public required Guid Id { get; init; }
        public required TicketType Type { get; init; }
        public required TicketStatus Status { get; init; }
        public required DateTime CreatedAtUtc { get; init; }
        public required string CustomerFullName { get; init; }
        public string? ProductName { get; init; }
        public string? ShortSummary { get; init; }
        public int? CustomerSatisfaction { get; init; }
        public int NumMessages { get; init; }
    }
}
