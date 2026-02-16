using ECommerce.Common.Application.Sorting;
using ECommerce.Modules.Ticketing.Domain.Customers;
using ECommerce.Modules.Ticketing.Domain.Products;
using ECommerce.Modules.Ticketing.Domain.Tickets;
using System.Linq.Expressions;
using static ECommerce.Modules.Ticketing.Application.Tickets.GetTickets.GetTicketsResponse;

namespace ECommerce.Modules.Ticketing.Application.Tickets;

public static class TicketMappings
{
    public static readonly SortMappingDefinition<GetTicketResponseItem, Ticket> SortMapping = new()
    {
        Mappings =
        [
            new SortMapping(nameof(GetTicketResponseItem.Id), nameof(Ticket.Id)),
            new SortMapping(nameof(GetTicketResponseItem.Type), nameof(Ticket.Type)),
            new SortMapping(nameof(GetTicketResponseItem.Status), nameof(Ticket.Status)),
            new SortMapping(nameof(GetTicketResponseItem.CreatedAtUtc), nameof(Ticket.CreatedAtUtc)),
            new SortMapping(nameof(GetTicketResponseItem.ShortSummary), nameof(Ticket.ShortSummary)),
            new SortMapping(nameof(GetTicketResponseItem.CustomerSatisfaction), nameof(Ticket.CustomerSatisfaction)),
            new SortMapping(nameof(GetTicketResponseItem.CustomerFullName), $"{nameof(Ticket.Customer)}.{nameof(Customer.FullName)}"),
            new SortMapping(nameof(GetTicketResponseItem.ProductName), $"{nameof(Ticket.Product)}.{nameof(Product.Name)}")
        ]
    };
}
