using ECommerce.Common.Application.Clock;
using ECommerce.Webhooks.Domain.Webhooks;
using ECommerce.Webhooks.Infrastructure.Database;
using MassTransit;
using System.Net.Http.Json;
using System.Text.Json;

namespace ECommerce.Webhooks.Infrastructure.Webhooks;

internal sealed class WebhookTriggerConsumer(
    IHttpClientFactory httpClientFactory,
    IDateTimeProvider dateTimeProvider,
    WebhooksDbContext dbContext)
    : IConsumer<WebhookTriggered>
{
    public async Task Consume(ConsumeContext<WebhookTriggered> context)
    {
        using var httpClient = httpClientFactory.CreateClient();

        WebhookTriggered webhookTriggered = context.Message;

        var payload = new WebhookPayload
        {
            Id = Guid.CreateVersion7(),
            EventType = webhookTriggered.EventType,
            SubscriptionId = webhookTriggered.SubscriptionId,
            Timestamp = dateTimeProvider.UtcNow,
            Data = webhookTriggered.Data
        };

        string jsonPayload = JsonSerializer.Serialize(payload);

        try
        {
            var response = await httpClient.PostAsJsonAsync(webhookTriggered.WebhookUrl, payload, context.CancellationToken);
            response.EnsureSuccessStatusCode();

            var attempt = new WebhookDeliveryAttempt
            {
                Id = Guid.CreateVersion7(),
                WebhookSubscriptionId = webhookTriggered.SubscriptionId,
                Payload = jsonPayload,
                ResponseStatusCode = (int)response.StatusCode,
                Success = response.IsSuccessStatusCode,
                Timestamp = dateTimeProvider.UtcNow,
            };

            dbContext.WebhookDeliveryAttempts.Add(attempt);

            await dbContext.SaveChangesAsync(context.CancellationToken);
        }
        catch (Exception)
        {
            var attempt = new WebhookDeliveryAttempt
            {
                Id = Guid.CreateVersion7(),
                WebhookSubscriptionId = webhookTriggered.SubscriptionId,
                Payload = jsonPayload,
                ResponseStatusCode = null,
                Success = false,
                Timestamp = DateTime.UtcNow,
            };

            dbContext.WebhookDeliveryAttempts.Add(attempt);

            await dbContext.SaveChangesAsync();
        }
    }
}
