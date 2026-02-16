namespace ECommerce.Webhooks.Application.Abstractions.Webhooks;

public interface IWebhookDispatcher
{
    Task DispatchAsync<T>(string eventType, T data, CancellationToken cancellationToken = default)
        where T : notnull;
}
