using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ECommerce.Modules.Ticketing.Domain.Products;

internal sealed class EmbeddingJsonConverter : JsonConverter<ReadOnlyMemory<float>>
{
    public override ReadOnlyMemory<float> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType != JsonTokenType.String)
        {
            throw new InvalidOperationException($"JSON deserialization failed because the value type was {reader.TokenType} but should be {JsonTokenType.String}");
        }


        byte[] bytes = reader.GetBytesFromBase64();
        ReadOnlySpan<float> floats = MemoryMarshal.Cast<byte, float>(bytes);
        return floats.ToArray(); // TODO: Can we avoid copying? The memory is already in the right format.
    }

    public override void Write(Utf8JsonWriter writer, ReadOnlyMemory<float> value, JsonSerializerOptions options)
    {
        ReadOnlySpan<byte> bytes = MemoryMarshal.AsBytes(value.Span);
        writer.WriteBase64StringValue(bytes);
    }
}
