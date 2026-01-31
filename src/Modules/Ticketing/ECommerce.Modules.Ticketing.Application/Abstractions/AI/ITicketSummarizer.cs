namespace ECommerce.Modules.Ticketing.Application.Abstractions.AI;

public interface ITicketSummarizer
{
    void UpdateSummary(Guid ticketId, bool enforceRateLimit);
}
