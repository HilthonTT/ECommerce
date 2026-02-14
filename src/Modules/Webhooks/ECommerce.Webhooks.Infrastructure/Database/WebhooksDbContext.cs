using ECommerce.Common.Infrastructure.Database;
using ECommerce.Webhooks.Application.Abstractions.Data;
using ECommerce.Webhooks.Domain.Webhooks;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Webhooks.Infrastructure.Database;

public sealed class WebhooksDbContext(DbContextOptions<WebhooksDbContext> options) : DbContext(options), IDbContext, IUnitOfWork
{
    public DbSet<WebhookSubscription> WebhookSubscriptions { get; set; }

    public DbSet<WebhookDeliveryAttempt> WebhookDeliveryAttempts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schemas.Webhooks);

        modelBuilder.ApplyConfigurationsFromAssembly(Common.Infrastructure.AssemblyReference.Assembly);
        modelBuilder.ApplyConfigurationsFromAssembly(AssemblyReference.Assembly);
    }
}
