using System.Text.Json.Serialization;

namespace EventBus.Events;

public record IntegrationEvent
{
    public IntegrationEvent()
    {
        Id = Guid.CreateVersion7();
        CreatedAtUtc = DateTime.UtcNow;
    }

    [JsonInclude]
    public Guid Id { get; }

    [JsonInclude]
    public DateTime CreatedAtUtc { get; }
}
