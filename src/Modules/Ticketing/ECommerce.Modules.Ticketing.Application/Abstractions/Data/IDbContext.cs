using ECommerce.Modules.Ticketing.Domain.Tickets;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Modules.Ticketing.Application.Abstractions.Data;

public interface IDbContext
{
    DbSet<Ticket> Tickets { get; }
}
