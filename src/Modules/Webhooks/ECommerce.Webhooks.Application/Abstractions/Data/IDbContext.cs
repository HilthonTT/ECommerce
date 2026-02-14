using ECommerce.Webhooks.Domain.Webhooks;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Webhooks.Application.Abstractions.Data;

public interface IDbContext
{
    DbSet<WebhookSubscription> WebhookSubscriptions { get; }
}
