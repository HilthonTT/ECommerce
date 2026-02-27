using ECommerce.Common.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ECommerce.Modules.Ticketing.Infrastructure.Database;

internal sealed class TicketingDbContextFactory : IDesignTimeDbContextFactory<TicketingDbContext>
{
    public TicketingDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TicketingDbContext>();

        optionsBuilder
            .UseNpgsql(
                "Host=localhost;Database=ecommerce;Username=postgres;Password=postgres",
                o => o.MigrationsHistoryTable("__EFMigrationsHistory", Schemas.Ticketing))
            .UseSnakeCaseNamingConvention();

        return new TicketingDbContext(optionsBuilder.Options);
    }
}
