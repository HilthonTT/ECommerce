using ECommerce.Webhooks.Application.Abstractions.Webhooks;
using ECommerce.Webhooks.Infrastructure.OpenTelemetry;
using MassTransit;
using System.Diagnostics;

namespace ECommerce.Webhooks.Infrastructure.Webhooks;

internal sealed class WebhookDispatcher(IPublishEndpoint publishEndpoint) : IWebhookDispatcher
{
    public async Task DispatchAsync<T>(string eventType, T data, CancellationToken cancellationToken = default)
        where T : notnull
    {
        using Activity? activity = DiagnosticConfig.Source.StartActivity($"{eventType} dispatch webhook");
        activity?.AddTag("event.type", eventType);

        WebhookDispatched message = new(eventType, data);
        await publishEndpoint.Publish(message, cancellationToken);
    }
}
