using ECommerce.Modules.Ticketing.Domain.Orders;
using ECommerce.Modules.Ticketing.Domain.Tickets;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Modules.Ticketing.Application.Abstractions.Data;

public interface IDbContext
{
    DbSet<Ticket> Tickets { get; }

    DbSet<Order> Orders { get; }
}
