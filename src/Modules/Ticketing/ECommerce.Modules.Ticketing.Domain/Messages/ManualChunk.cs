using ECommerce.Common.Domain;
using ECommerce.Common.Domain.Auditing;

namespace ECommerce.Modules.Ticketing.Domain.Messages;

[Auditable]
public sealed class ManualChunk : Entity
{
    public Guid Id { get; private init; }

    public Guid ProductId { get; private set; }

    public int PageNumber { get; private set; }

    public required string Text { get; set; } = string.Empty;

    public required byte[] Embedding { get; set; }

    public static ManualChunk Create(Guid productId, int pageNumber, string text, byte[] embedding)
    {
        return new ManualChunk
        {
            Id = Guid.CreateVersion7(),
            ProductId = productId,
            PageNumber = pageNumber,
            Text = text,
            Embedding = embedding,
        };
    }
}
