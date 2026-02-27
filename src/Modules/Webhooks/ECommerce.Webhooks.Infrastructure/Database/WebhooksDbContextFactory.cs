using ECommerce.Common.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ECommerce.Webhooks.Infrastructure.Database;

internal sealed class WebhooksDbContextFactory : IDesignTimeDbContextFactory<WebhooksDbContext>
{
    public WebhooksDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<WebhooksDbContext>();

        optionsBuilder
            .UseNpgsql(
                "Host=localhost;Database=ecommerce;Username=postgres;Password=postgres",
                o => o.MigrationsHistoryTable("__EFMigrationsHistory", Schemas.Webhooks))
            .UseSnakeCaseNamingConvention();

        return new WebhooksDbContext(optionsBuilder.Options);
    }
}