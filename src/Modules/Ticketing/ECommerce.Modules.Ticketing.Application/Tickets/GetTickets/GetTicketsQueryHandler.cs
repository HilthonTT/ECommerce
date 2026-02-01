using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Application.Sorting;
using ECommerce.Common.Domain;
using ECommerce.Modules.Ticketing.Application.Abstractions.Data;
using ECommerce.Modules.Ticketing.Domain.Tickets;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Modules.Ticketing.Application.Tickets.GetTickets;

internal sealed class GetTicketsQueryHandler(IDbContext dbContext, ISortMappingProvider sortMappingProvider) 
    : IQueryHandler<GetTicketsQuery, GetTicketsResponse>
{
    public async Task<Result<GetTicketsResponse>> Handle(
        GetTicketsQuery query, 
        CancellationToken cancellationToken)
    {
        IQueryable<Ticket> ticketsQuery = dbContext.Tickets
            .Include(t => t.Product)
            .AsQueryable();

        if (query.BrandIds.Count > 0)
        {
            ticketsQuery = ticketsQuery
                .Where(t => t.Product != null)
                .Where(t => query.BrandIds.Contains(t.Product!.ProductBrandId));
        }

        if (query.CustomerId != Guid.Empty)
        {
            ticketsQuery = ticketsQuery
                .Where(t => t.CustomerId == query.CustomerId);
        }

        Dictionary<TicketStatus, int> ticketsQueryCountByStatus = await ticketsQuery.GroupBy(t => t.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToDictionaryAsync(g => g.Status, g => g.Count, cancellationToken);

        int totalOpen = ticketsQueryCountByStatus.GetValueOrDefault(TicketStatus.Open);
        int totalClosed = ticketsQueryCountByStatus.GetValueOrDefault(TicketStatus.Closed);

        if (query.Status is not null)
        {
            ticketsQuery = ticketsQuery.Where(t => t.Status == query.Status);
        }

        SortMapping[] sortMappings = sortMappingProvider.GetMappings<GetTicketsResponse.GetTicketResponseItem, Ticket>();

        ticketsQuery = ticketsQuery
            .AsNoTracking()
            .ApplySort(query.Sort, sortMappings);

        List<GetTicketsResponse.GetTicketResponseItem> resultItems = await ticketsQuery
            .Skip(query.StartIndex)
            .Take(query.MaxResults)
            .Select(t => new GetTicketsResponse.GetTicketResponseItem
            {
                Id = t.Id,
                Type = t.Type,
                Status = t.Status,
                CreatedAtUtc = t.CreatedAtUtc,
                CustomerFullName = t.Customer.FullName,
                ProductName = t.Product != null ? t.Product.Name : null,
                ShortSummary = t.ShortSummary,
                CustomerSatisfaction = t.CustomerSatisfaction,
                NumMessages = t.Messages.Count,
            })
            .ToListAsync(cancellationToken);

        int totalCount = await ticketsQuery.CountAsync(cancellationToken);

        return new GetTicketsResponse(resultItems, totalCount, totalOpen, totalClosed);
    }
}
