using ECommerce.Modules.Catalog.Infrastructure.Database;
using ECommerce.Modules.Ticketing.Infrastructure.Database;
using ECommerce.Modules.Users.Infrastructure.Database;
using ECommerce.Webhooks.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Extensions;

internal static class DatabaseExtensions
{
    internal static async Task ApplyMigrationsAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();

        await ApplyMigrationsAsync<CatalogDbContext>(scope);
        await ApplyMigrationsAsync<WebhooksDbContext>(scope);
        await ApplyMigrationsAsync<UsersDbContext>(scope);
        await ApplyMigrationsAsync<TicketingDbContext>(scope);
    }

    private static async Task ApplyMigrationsAsync<TDbContext>(IServiceScope scope)
        where TDbContext : DbContext
    {
        using var context = scope.ServiceProvider.GetRequiredService<TDbContext>();

        await context.Database.MigrateAsync();
    }
}
