using ECommerce.Common.Domain;
using ECommerce.Common.Domain.Auditing;
using ECommerce.Modules.Ticketing.Domain.Customers;
using ECommerce.Modules.Ticketing.Domain.Messages;
using ECommerce.Modules.Ticketing.Domain.Products;

namespace ECommerce.Modules.Ticketing.Domain.Tickets;

[Auditable]
public sealed class Ticket : Entity
{
    public Guid Id { get; private init; }

    public int? ProductId { get; private set; }

    public Guid CustomerId { get; private init; }

    /// <summary>
    /// Gets or sets the customer associated with this entity.
    /// Used for EFCore Mapping
    /// </summary>
    public Customer Customer { get; set; } = default!;

    /// <summary>
    /// Gets or sets the customer associated with this entity.
    /// Used for EFCore Mapping
    /// </summary>
    public Product Product { get; set; } = default!;

    public DateTime CreatedAtUtc { get; private init; }

    public string? ShortSummary { get; private set; }

    public string? LongSummary { get; private set; }

    public int? CustomerSatisfaction { get; private set; }

    public string Code { get; private init; } = string.Empty;

    public TicketStatus Status { get; private set; }

    public TicketType Type { get; private set; }

    public bool Archived { get; private set; }

    public List<Message> Messages { get; set; } = [];

    private Ticket() { }

    public static Result<Ticket> Create(
        Guid customerId,
        TicketType ticketType,
        int? productId = null)
    {
        if (customerId == Guid.Empty)
        {
            return Result.Failure<Ticket>(TicketErrors.CustomerIdCannotBeEmpty);
        }

        var ticket = new Ticket
        {
            Id = Guid.CreateVersion7(),
            CustomerId = customerId,
            ProductId = productId,
            Code = GenerateTicketCode(),
            CreatedAtUtc = DateTime.UtcNow,
            Type = ticketType,
            Status = TicketStatus.Open
        };

        ticket.RaiseDomainEvent(new TicketCreatedDomainEvent(ticket.Id));

        return Result.Success(ticket);
    }

    public Result UpdateSummary(string? shortSummary, string? longSummary)
    {
        if (Archived)
        {
            return TicketErrors.CannotUpdateArchivedTicket;
        }

        ShortSummary = shortSummary;
        LongSummary = longSummary;

        return Result.Success();
    }

    public void UpdateInfo(int? productId, TicketType type, TicketStatus status)
    {
        ProductId = productId;
        Type = type;
        Status = status;
    }

    public Result SetCustomerSatisfaction(int satisfaction)
    {
        if (satisfaction < 1 || satisfaction > 5)
        {
            return TicketErrors.SatisfactionMustBeBetweenOneAndFive;
        }

        if (Status != TicketStatus.Closed)
        {
            return TicketErrors.CannotSetSatisfactionOnOpenTicket;
        }

        CustomerSatisfaction = satisfaction;

        return Result.Success();
    }

    public Result Close()
    {
        if (Archived)
        {
            return TicketErrors.CannotCloseArchivedTicket;
        }

        if (Status == TicketStatus.Closed)
        {
            return TicketErrors.TicketAlreadyClosed;
        }

        Status = TicketStatus.Closed;
        RaiseDomainEvent(new TicketClosedDomainEvent(Id, Code));

        return Result.Success();
    }

    public Result Archive()
    {
        if (Archived)
        {
            return TicketErrors.TicketAlreadyArchived;
        }

        if (Status != TicketStatus.Closed)
        {
            return TicketErrors.OnlyClosedTicketsCanBeArchived;
        }

        Archived = true;
        RaiseDomainEvent(new TicketArchivedDomainEvent(Id, Code));

        return Result.Success();
    }

    private static string GenerateTicketCode()
    {
        return $"TC-{Guid.CreateVersion7().ToString()[..8].ToUpperInvariant()}";
    }
}
