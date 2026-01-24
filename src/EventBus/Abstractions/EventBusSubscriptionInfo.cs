using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace EventBus.Abstractions;

public sealed class EventBusSubscriptionInfo
{
    private static readonly JsonSerializerOptions DefaultSerializerOptions = new()
    {
        TypeInfoResolver = JsonSerializer.IsReflectionEnabledByDefault 
            ? new DefaultJsonTypeInfoResolver()
            : JsonTypeInfoResolver.Combine()
    };

    public Dictionary<string, Type> EventTypes { get; } = [];

    public JsonSerializerOptions JsonSerializerOptions { get; } = new();
}
