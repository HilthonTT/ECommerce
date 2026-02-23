using Microsoft.EntityFrameworkCore;

namespace ECommerce.Common.Infrastructure.Database;

public interface IDbSeeder<in TContext> where TContext : DbContext
{
    Task SeedAsync(TContext context);
}
