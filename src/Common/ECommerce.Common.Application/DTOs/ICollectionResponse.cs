namespace ECommerce.Common.Application.DTOs;

public interface ICollectionResponse<T>
{
    List<T> Items { get; init; }
}
