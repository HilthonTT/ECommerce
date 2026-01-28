using ECommerce.Common.Domain;

namespace ECommerce.Modules.Ticketing.Domain.Tickets;

public static class TicketErrors
{
    public static readonly Error CustomerIdCannotBeEmpty = Error.Problem(
        "Ticket.CustomerIdCannotBeEmpty",
        "Customer ID cannot be empty.");

    public static readonly Error SatisfactionMustBeBetweenOneAndFive = Error.Problem(
        "Ticket.SatisfactionMustBeBetweenOneAndFive",
        "Customer satisfaction must be between 1 and 5.");

    public static readonly Error CannotUpdateArchivedTicket = Error.Problem(
        "Ticket.CannotUpdateArchivedTicket",
        "Cannot update an archived ticket.");

    public static readonly Error CannotSetSatisfactionOnOpenTicket = Error.Problem(
        "Ticket.CannotSetSatisfactionOnOpenTicket",
        "Cannot set satisfaction on a ticket that is not closed.");

    public static readonly Error CannotCloseArchivedTicket = Error.Problem(
        "Ticket.CannotCloseArchivedTicket",
        "Cannot close an archived ticket.");

    public static readonly Error TicketAlreadyClosed = Error.Problem(
        "Ticket.TicketAlreadyClosed",
        "Ticket is already closed.");

    public static readonly Error TicketAlreadyArchived = Error.Problem(
        "Ticket.TicketAlreadyArchived",
        "Ticket is already archived.");

    public static readonly Error OnlyClosedTicketsCanBeArchived = Error.Problem(
        "Ticket.OnlyClosedTicketsCanBeArchived",
        "Only closed tickets can be archived.");
}
