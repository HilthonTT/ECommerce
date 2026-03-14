namespace ECommerce.Web.Shared.DTOs.Common;

public sealed class CollectionResponse<T>
{
    public List<T> Items { get; init; } = [];
}
