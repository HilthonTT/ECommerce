namespace ECommerce.Common.Application.DTOs;

public sealed class CollectionResponse<T> : ICollectionResponse<T>, ILinksResponse
{
    public List<T> Items { get; init; } = [];
    public List<LinkDto> Links { get; init; } = [];
}
