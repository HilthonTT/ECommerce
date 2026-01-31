using ECommerce.Modules.Ticketing.Domain.Customers;
using ECommerce.Modules.Ticketing.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Modules.Ticketing.Infrastructure.Customers;

internal sealed class CustomerRepository(TicketingDbContext dbContext) : ICustomerRepository
{
    public Task<Customer?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return dbContext.Customers.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public void Insert(Customer customer)
    {
        dbContext.Customers.Add(customer);
    }
}
