using ECommerce.Common.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ECommerce.Modules.Catalog.Infrastructure.Database;

internal sealed class CatalogDbContextFactory : IDesignTimeDbContextFactory<CatalogDbContext>
{
    public CatalogDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<CatalogDbContext>();

        optionsBuilder
            .UseNpgsql(
                "Host=localhost;Database=ecommerce;Username=postgres;Password=postgres",
                o => o.MigrationsHistoryTable("__EFMigrationsHistory", Schemas.Catalog))
            .UseSnakeCaseNamingConvention();

        return new CatalogDbContext(optionsBuilder.Options);
    }
}