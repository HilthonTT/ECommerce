using ECommerce.Common.Application.DTOs;
using ECommerce.Common.Application.Messaging;

namespace ECommerce.Webhooks.Application.Webhooks.GetSubscriptions;

public sealed record GetSubscriptionsQuery(Guid UserId, string? EventType, int Page, int PageSize) 
    : IQuery<PaginationResult<WebhookSubscriptionResponse>>;
