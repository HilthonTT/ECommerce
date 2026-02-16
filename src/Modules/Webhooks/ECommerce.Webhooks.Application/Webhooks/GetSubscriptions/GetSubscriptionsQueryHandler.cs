using ECommerce.Common.Application.DTOs;
using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Application.Sorting;
using ECommerce.Common.Domain;
using ECommerce.Webhooks.Application.Abstractions.Data;
using ECommerce.Webhooks.Domain.Webhooks;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Webhooks.Application.Webhooks.GetSubscriptions;

internal sealed class GetSubscriptionsQueryHandler(IDbContext dbContext, ISortMappingProvider sortMappingProvider)
    : IQueryHandler<GetSubscriptionsQuery, PaginationResult<WebhookSubscriptionResponse>>
{
    public async Task<Result<PaginationResult<WebhookSubscriptionResponse>>> Handle(
        GetSubscriptionsQuery query, 
        CancellationToken cancellationToken)
    {
        SortMapping[] sortMappings = sortMappingProvider.GetMappings<WebhookSubscriptionResponse, WebhookSubscription>();

        IQueryable<WebhookSubscriptionResponse> subscriptionsQuery = dbContext.WebhookSubscriptions
            .AsNoTracking()
            .Where(s => s.UserId == query.UserId)
            .ApplySort(query.Sort, sortMappings)
            .Select(WebhookSubscriptionMappings.ProjectToResponse());

        var paginationResult = await PaginationResult<WebhookSubscriptionResponse>.CreateAsync(
            subscriptionsQuery,
            query.Page,
            query.PageSize,
            cancellationToken);

        return paginationResult;
    }
}
