using EventBus.Abstractions;
using EventBus.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace EventBus.RabbitMQ;

internal sealed class RabbitMQEventBus(
    ILogger<RabbitMQEventBus> logger,
    IServiceProvider serviceProvider,
    IOptions<EventBusOptions> options,
    IOptions<EventBusSubscriptionInfo> subscriptionOptions,
    RabbitMQTelemetry rabbitMQTelemetry) : IEventBus, IDisposable, IHostedService
{
    private const string ExchangeName = "eshop_event_bus";
    private const string DeadLetterExchangeName = "eshop_event_bus_dlx";
    private const string DeadLetterQueueSuffix = "_dlq";
    private const int MaxRetryAttempts = 3;
    private const string RetryCountHeader = "x-retry-count";

    private readonly ResiliencePipeline _pipeline = CreateResiliencePipeline(options.Value.RetryCount);
    private readonly TextMapPropagator _propagator = rabbitMQTelemetry.Propagator;
    private readonly ActivitySource _activitySource = rabbitMQTelemetry.ActivitySource;
    private readonly string _queueName = options.Value.SubscriptionClientName;
    private readonly string _deadLetterQueueName = $"{options.Value.SubscriptionClientName}{DeadLetterQueueSuffix}";
    private readonly EventBusSubscriptionInfo _subscriptionInfo = subscriptionOptions.Value;

    private IConnection? _rabbitMQConnection;
    private IChannel? _consumerChannel;
    private readonly SemaphoreSlim _channelLock = new(1, 1);

    public async Task PublishAsync(IntegrationEvent @event)
    {
        ArgumentNullException.ThrowIfNull(@event);

        string routingKey = @event.GetType().Name;

        if (logger.IsEnabled(LogLevel.Trace))
        {
            logger.LogTrace("Creating RabbitMQ channel to publish event: {EventId} ({EventName})", @event.Id, routingKey);
        }

        if (_rabbitMQConnection is null || !_rabbitMQConnection.IsOpen)
        {
            throw new InvalidOperationException("RabbitMQ connection is not open");
        }

        await using IChannel channel = await _rabbitMQConnection.CreateChannelAsync();

        if (logger.IsEnabled(LogLevel.Trace))
        {
            logger.LogTrace("Declaring RabbitMQ exchange to publish event: {EventId}", @event.Id);
        }

        await channel.ExchangeDeclareAsync(
            exchange: ExchangeName,
            type: "direct",
            durable: true,
            autoDelete: false);

        byte[] body = SerializeMessage(@event);

        // Start an activity with a name following the semantic convention of the OpenTelemetry messaging specification.
        // https://github.com/open-telemetry/semantic-conventions/blob/main/docs/messaging/messaging-spans.md
        string activityName = $"{routingKey} publish";

        await _pipeline.ExecuteAsync(async (cancellationToken) =>
        {
            using Activity? activity = _activitySource.StartActivity(activityName, ActivityKind.Client);

            // Depending on Sampling (and whether a listener is registered or not), the activity above may not be created.
            // If it is created, then propagate its context. If it is not created, the propagate the Current context, if any.
            ActivityContext contextToInject = default;

            if (activity is not null)
            {
                contextToInject = activity.Context;
            }
            else if (Activity.Current is not null)
            {
                contextToInject = Activity.Current.Context;
            }

            var properties = new BasicProperties()
            {
                DeliveryMode = DeliveryModes.Persistent,
                ContentType = "application/json",
                ContentEncoding = "utf-8",
                MessageId = @event.Id.ToString(),
                Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds()),
                AppId = _queueName
            };

            _propagator.Inject(
                new PropagationContext(contextToInject, Baggage.Current),
                properties,
                InjectTraceContextIntoBasicProperties);

            SetActivityContext(activity, routingKey, "publish");

            if (logger.IsEnabled(LogLevel.Trace))
            {
                logger.LogTrace("Publishing event to RabbitMQ: {EventId}", @event.Id);
            }

            try
            {
                await channel.BasicPublishAsync(
                    exchange: ExchangeName,
                    routingKey: routingKey,
                    mandatory: true,
                    basicProperties: properties,
                    body: body,
                    cancellationToken: cancellationToken);

                if (logger.IsEnabled(LogLevel.Debug))
                {
                    logger.LogDebug("Event {EventId} published and confirmed", @event.Id);
                }
            }
            catch (Exception ex)
            {
                activity?.SetExceptionTags(ex);
                logger.LogError(ex, "Failed to publish event {EventId} to RabbitMQ", @event.Id);
                throw;
            }
        }, CancellationToken.None);

        static void InjectTraceContextIntoBasicProperties(IBasicProperties props, string key, string value)
        {
            props.Headers ??= new Dictionary<string, object?>();
            props.Headers[key] = value;
        }
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Messaging is async so we don't need to wait for it to complete.
        _ = Task.Run(async () =>
        {
            try
            {
                logger.LogInformation("Starting RabbitMQ connection on a background thread");

                _rabbitMQConnection = serviceProvider.GetRequiredService<IConnection>();
                if (!_rabbitMQConnection.IsOpen)
                {
                    logger.LogError("RabbitMQ connection failed to open");
                    return;
                }

                // Setup connection recovery
                _rabbitMQConnection.ConnectionShutdownAsync += OnConnectionShutdown;
                _rabbitMQConnection.CallbackExceptionAsync += OnCallbackException;
                _rabbitMQConnection.ConnectionBlockedAsync += OnConnectionBlocked;

                await InitializeChannelAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error starting RabbitMQ connection");
            }
        }, cancellationToken);

        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Stopping RabbitMQ event bus");

        if (_consumerChannel is not null)
        {
            try
            {
                await _consumerChannel.CloseAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Error closing consumer channel");
            }
        }

        if (_rabbitMQConnection is not null)
        {
            try
            {
                await _rabbitMQConnection.CloseAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Error closing RabbitMQ connection");
            }
        }
    }

    public void Dispose()
    {
        _consumerChannel?.Dispose();
        _channelLock.Dispose();
    }

    private async Task InitializeChannelAsync()
    {
        await _channelLock.WaitAsync();
        try
        {
            if (_consumerChannel is not null && _consumerChannel.IsOpen)
            {
                return;
            }

            if (_rabbitMQConnection is null || !_rabbitMQConnection.IsOpen)
            {
                logger.LogError("Cannot initialize channel: RabbitMQ connection is not open");
                return;
            }

            if (logger.IsEnabled(LogLevel.Trace))
            {
                logger.LogTrace("Creating RabbitMQ consumer channel");
            }

            _consumerChannel = await _rabbitMQConnection.CreateChannelAsync();

            // Set prefetch count to limit concurrent message processing
            await _consumerChannel.BasicQosAsync(prefetchSize: 0, prefetchCount: 10, global: false);

            _consumerChannel.CallbackExceptionAsync += (sender, ea) =>
            {
                logger.LogWarning(ea.Exception, "Error with RabbitMQ consumer channel");
                return Task.CompletedTask;
            };

            // Declare main exchange
            await _consumerChannel.ExchangeDeclareAsync(
                exchange: ExchangeName,
                type: "direct",
                durable: true,
                autoDelete: false);

            // Declare Dead Letter Exchange
            await _consumerChannel.ExchangeDeclareAsync(
                exchange: DeadLetterExchangeName,
                type: "direct",
                durable: true,
                autoDelete: false);

            // Declare main queue with DLX configuration
            var queueArguments = new Dictionary<string, object?>
            {
                ["x-dead-letter-exchange"] = DeadLetterExchangeName,
                ["x-dead-letter-routing-key"] = _deadLetterQueueName
            };

            await _consumerChannel.QueueDeclareAsync(
                queue: _queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: queueArguments);

            // Declare Dead Letter Queue
            await _consumerChannel.QueueDeclareAsync(
                queue: _deadLetterQueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            // Bind DLQ to DLX
            await _consumerChannel.QueueBindAsync(
                queue: _deadLetterQueueName,
                exchange: DeadLetterExchangeName,
                routingKey: _deadLetterQueueName);

            if (logger.IsEnabled(LogLevel.Trace))
            {
                logger.LogTrace("Starting RabbitMQ basic consume");
            }

            var consumer = new AsyncEventingBasicConsumer(_consumerChannel);
            consumer.ReceivedAsync += OnMessageReceived;

            await _consumerChannel.BasicConsumeAsync(
                queue: _queueName,
                autoAck: false,
                consumer: consumer);

            // Bind queue to exchange for all subscribed event types
            foreach (var (eventName, _) in _subscriptionInfo.EventTypes)
            {
                await _consumerChannel.QueueBindAsync(
                    queue: _queueName,
                    exchange: ExchangeName,
                    routingKey: eventName);

                logger.LogInformation("Bound queue {QueueName} to event {EventName}", _queueName, eventName);
            }

            logger.LogInformation("RabbitMQ consumer channel initialized successfully");
        }
        finally
        {
            _channelLock.Release();
        }
    }

    private async Task OnMessageReceived(object sender, BasicDeliverEventArgs eventArgs)
    {
        // Extract the PropagationContext of the upstream parent from the message headers.
        var parentContext = _propagator.Extract(default, eventArgs.BasicProperties, ExtractTraceContextFromBasicProperties);
        Baggage.Current = parentContext.Baggage;

        // Start an activity with a name following the semantic convention of the OpenTelemetry messaging specification.
        // https://github.com/open-telemetry/semantic-conventions/blob/main/docs/messaging/messaging-spans.md
        var activityName = $"{eventArgs.RoutingKey} receive";

        using var activity = _activitySource.StartActivity(activityName, ActivityKind.Consumer, parentContext.ActivityContext);

        SetActivityContext(activity, eventArgs.RoutingKey, "receive");

        string eventName = eventArgs.RoutingKey;
        string message = Encoding.UTF8.GetString(eventArgs.Body.Span);

        // Get retry count from headers
        int retryCount = GetRetryCount(eventArgs.BasicProperties);

        try
        {
            activity?.SetTag("message", message);
            activity?.SetTag("retry_count", retryCount);

            if (message.Contains("throw-fake-exception", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new InvalidOperationException($"Fake exception requested: \"{message}\"");
            }

            await ProcessEvent(eventName, message);

            // Successfully processed - acknowledge the message
            if (_consumerChannel is not null)
            {
                await _consumerChannel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);

                if (logger.IsEnabled(LogLevel.Debug))
                {
                    logger.LogDebug("Message acknowledged: {EventName}", eventName);
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing message \"{Message}\" (Retry: {RetryCount}/{MaxRetries})",
                message, retryCount, MaxRetryAttempts);

            activity?.SetExceptionTags(ex);

            await HandleFailedMessage(eventArgs, retryCount, ex);
        }

        static IEnumerable<string> ExtractTraceContextFromBasicProperties(IReadOnlyBasicProperties props, string key)
        {
            if (props.Headers is null)
            {
                return [];
            }

            if (props.Headers.TryGetValue(key, out object? value))
            {
                byte[]? bytes = value as byte[];
                return [Encoding.UTF8.GetString(bytes ?? [])];
            }

            return [];
        }
    }

    private async Task HandleFailedMessage(BasicDeliverEventArgs eventArgs, int retryCount, Exception exception)
    {
        if (_consumerChannel is null)
        {
            return;
        }

        if (retryCount < MaxRetryAttempts)
        {
            // Reject and requeue with incremented retry count
            logger.LogWarning("Requeuing message for retry {RetryCount}/{MaxRetries}",
                retryCount + 1, MaxRetryAttempts);

            // Increment retry count in headers
            var headers = eventArgs.BasicProperties.Headers ?? new Dictionary<string, object?>();
            headers[RetryCountHeader] = retryCount + 1;
            headers["x-first-death-reason"] = exception.Message;
            headers["x-first-death-queue"] = _queueName;
            headers["x-first-death-exchange"] = ExchangeName;

            var newProperties = new BasicProperties
            {
                Headers = headers,
                DeliveryMode = eventArgs.BasicProperties.DeliveryMode,
                ContentType = eventArgs.BasicProperties.ContentType,
                ContentEncoding = eventArgs.BasicProperties.ContentEncoding,
                MessageId = eventArgs.BasicProperties.MessageId,
                CorrelationId = eventArgs.BasicProperties.CorrelationId,
                Timestamp = eventArgs.BasicProperties.Timestamp,
                AppId = eventArgs.BasicProperties.AppId
            };

            // Reject the message (it will go to DLX due to queue configuration)
            // But first publish it again with updated retry count
            await _consumerChannel.BasicPublishAsync(
                exchange: ExchangeName,
                routingKey: eventArgs.RoutingKey,
                mandatory: true,
                basicProperties: newProperties,
                body: eventArgs.Body);

            // Acknowledge the original message
            await _consumerChannel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
        }
        else
        {
            // Max retries exceeded - send to DLQ
            logger.LogError("Max retries exceeded for message. Sending to Dead Letter Queue");

            // Reject without requeue - message will be routed to DLQ
            await _consumerChannel.BasicNackAsync(
                deliveryTag: eventArgs.DeliveryTag,
                multiple: false,
                requeue: false);
        }
    }

    private static int GetRetryCount(IReadOnlyBasicProperties properties)
    {
        if (properties.Headers is null || !properties.Headers.TryGetValue(RetryCountHeader, out var value))
        {
            return 0;
        }

        return value switch
        {
            int intValue => intValue,
            byte byteValue => byteValue,
            short shortValue => shortValue,
            long longValue => (int)longValue,
            _ => 0
        };
    }

    private Task OnConnectionShutdown(object? sender, ShutdownEventArgs args)
    {
        logger.LogWarning("RabbitMQ connection shutdown: {ReplyCode} - {ReplyText}",
            args.ReplyCode, args.ReplyText);

        return Task.CompletedTask;
    }

    private Task OnCallbackException(object? sender, CallbackExceptionEventArgs args)
    {
        logger.LogError(args.Exception, "RabbitMQ callback exception");
        return Task.CompletedTask;
    }

    private Task OnConnectionBlocked(object? sender, ConnectionBlockedEventArgs args)
    {
        logger.LogWarning("RabbitMQ connection blocked: {Reason}", args.Reason);
        return Task.CompletedTask;
    }

    private static void SetActivityContext(Activity? activity, string routingKey, string operation)
    {
        if (activity is not null)
        {
            // These tags are added demonstrating the semantic conventions of the OpenTelemetry messaging specification
            // https://github.com/open-telemetry/semantic-conventions/blob/main/docs/messaging/messaging-spans.md
            activity.SetTag("messaging.system", "rabbitmq");
            activity.SetTag("messaging.destination_kind", "queue");
            activity.SetTag("messaging.operation", operation);
            activity.SetTag("messaging.destination.name", routingKey);
            activity.SetTag("messaging.rabbitmq.routing_key", routingKey);
        }
    }

    private async Task ProcessEvent(string eventName, string message)
    {
        if (logger.IsEnabled(LogLevel.Trace))
        {
            logger.LogTrace("Processing RabbitMQ event: {EventName}", eventName);
        }

        await using var scope = serviceProvider.CreateAsyncScope();

        if (!_subscriptionInfo.EventTypes.TryGetValue(eventName, out var eventType))
        {
            logger.LogWarning("Unable to resolve event type for event name {EventName}", eventName);
            throw new InvalidOperationException($"No event type registered for event name: {eventName}");
        }

        // Deserialize the event
        IntegrationEvent? integrationEvent = DeserializeMessage(message, eventType)
            ?? throw new InvalidOperationException($"Failed to deserialize event: {eventName}");

        // Get all the handlers using the event type as the key
        IEnumerable<IIntegrationEventHandler> handlers =
            scope.ServiceProvider.GetKeyedServices<IIntegrationEventHandler>(eventType);

        var handlersList = handlers.ToList();

        if (handlersList.Count == 0)
        {
            logger.LogWarning("No handlers registered for event type {EventType}", eventType.Name);
            return;
        }

        // Process handlers sequentially to maintain order
        foreach (IIntegrationEventHandler handler in handlersList)
        {
            string handlerType = handler.GetType().Name;

            if (logger.IsEnabled(LogLevel.Debug))
            {
                logger.LogDebug("Executing handler {HandlerType} for event {EventName}", handlerType, eventName);
            }

            try
            {
                await handler.Handle(integrationEvent);

                if (logger.IsEnabled(LogLevel.Debug))
                {
                    logger.LogDebug("Handler {HandlerType} completed successfully", handlerType);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Handler {HandlerType} failed for event {EventName}", handlerType, eventName);
                throw;
            }
        }
    }

    private byte[] SerializeMessage(IntegrationEvent @event)
    {
        try
        {
            return JsonSerializer.SerializeToUtf8Bytes(@event, @event.GetType(), _subscriptionInfo.JsonSerializerOptions);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to serialize event {EventType}", @event.GetType().Name);
            throw;
        }
    }

    private IntegrationEvent? DeserializeMessage(string message, Type eventType)
    {
        try
        {
            return JsonSerializer.Deserialize(message, eventType, _subscriptionInfo.JsonSerializerOptions) as IntegrationEvent;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to deserialize message to type {EventType}", eventType.Name);
            throw;
        }
    }

    private static ResiliencePipeline CreateResiliencePipeline(int retryCount)
    {
        var retryOptions = new RetryStrategyOptions
        {
            ShouldHandle = new PredicateBuilder()
                .Handle<BrokerUnreachableException>()
                .Handle<SocketException>()
                .Handle<AlreadyClosedException>(),
            MaxRetryAttempts = retryCount,
            DelayGenerator = (context) => ValueTask.FromResult(GenerateDelay(context.AttemptNumber)),
            BackoffType = DelayBackoffType.Exponential,
            UseJitter = true
        };

        return new ResiliencePipelineBuilder()
            .AddRetry(retryOptions)
            .Build();

        static TimeSpan? GenerateDelay(int attempt)
        {
            return TimeSpan.FromSeconds(Math.Pow(2, attempt));
        }
    }
}
