using ECommerce.Common.Application.DTOs;
using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Domain;
using ECommerce.Webhooks.Application.Abstractions.Data;

namespace ECommerce.Webhooks.Application.Webhooks.GetSubscriptions;

internal sealed class GetSubscriptionsQueryHandler(IDbContext dbContext)
    : IQueryHandler<GetSubscriptionsQuery, PaginationResult<WebhookSubscriptionResponse>>
{
    public async Task<Result<PaginationResult<WebhookSubscriptionResponse>>> Handle(
        GetSubscriptionsQuery query, 
        CancellationToken cancellationToken)
    {
        var subscriptionsQuery = dbContext.WebhookSubscriptions
            .Where(s => s.UserId == query.UserId);

        // TODO: Implement this handler
    }
}
