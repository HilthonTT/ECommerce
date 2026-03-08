namespace ECommerce.Common.Application.DTOs;

public sealed class CollectionResponse<T> : ICollectionResponse<T>
{
    public List<T> Items { get; init; } = [];
}
