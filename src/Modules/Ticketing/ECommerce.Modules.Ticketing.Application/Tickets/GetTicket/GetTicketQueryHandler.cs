using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Domain;
using ECommerce.Modules.Ticketing.Application.Abstractions.Data;
using ECommerce.Modules.Ticketing.Domain.Tickets;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Modules.Ticketing.Application.Tickets.GetTicket;

internal sealed class GetTicketQueryHandler(IDbContext dbContext) 
    : IQueryHandler<GetTicketQuery, TicketDetailsResult>
{
    public async Task<Result<TicketDetailsResult>> Handle(
        GetTicketQuery query, 
        CancellationToken cancellationToken)
    {
        Ticket? ticket = await dbContext.Tickets
            .AsNoTracking()
            .Where(t => query.CustomerId == Guid.Empty || t.CustomerId == query.CustomerId)
            .Include(t => t.Messages)
            .Include(t => t.Customer)
            .FirstOrDefaultAsync(t => t.Id == query.TicketId, cancellationToken);

        if (ticket is null)
        {
            return TicketErrors.NotFound(query.TicketId);
        }

        return new TicketDetailsResult(
           Id: ticket.Id,
           CreatedAtUtc: ticket.CreatedAtUtc,
           CustomerId: ticket.CustomerId,
           CustomerFullName: ticket.Customer.FullName,
           ShortSummary: ticket.ShortSummary,
           LongSummary: ticket.LongSummary,
           ProductId: ticket.ProductId,
           ProductBrand: ticket.Product?.ProductBrand.Brand,
           ProductName: ticket.Product?.Name,
           Type: ticket.Type,
           Status: ticket.Status,
           CustomerSatisfaction: ticket.CustomerSatisfaction,
           Messages: ticket.Messages
               .OrderBy(m => m.CreatedAtUtc)
               .Select(m => new TicketDetailsResult.TicketDetailsResultMessage(
                   MessageId: m.Id,
                   CreatedAtUtc: m.CreatedAtUtc,
                   IsCustomerMessage: m.IsCustomerMessage,
                   MessageText: m.Text))
               .ToList());
    }
}
