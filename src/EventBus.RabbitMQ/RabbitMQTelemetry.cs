using OpenTelemetry.Context.Propagation;
using System.Diagnostics;

namespace EventBus.RabbitMQ;

public sealed class RabbitMQTelemetry
{
    public const string ActivitySourceName = "EventBusRabbitMQ";

    public ActivitySource ActivitySource { get; } = new(ActivitySourceName);

    public TextMapPropagator Propagator { get; } = Propagators.DefaultTextMapPropagator;
}