using ECommerce.Modules.Ticketing.Domain.Orders;
using ECommerce.Modules.Ticketing.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Modules.Ticketing.Infrastructure.Orders;

internal sealed class OrderRepository(TicketingDbContext dbContext) : IOrderRepository
{
    public Task<Order?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return dbContext.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public void Insert(Order order)
    {
        dbContext.Orders.Add(order);
    }

    public void Remove(Order order)
    {
        dbContext.Orders.Remove(order);
    }
}
