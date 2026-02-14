using Microsoft.EntityFrameworkCore;

namespace ECommerce.Common.Application.DTOs;

public sealed record PaginationResult<T> : ICollectionResponse<T>
{
    private PaginationResult()
    {
    }

    public List<T> Items { get; init; } = [];
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;

    public static async Task<PaginationResult<T>> CreateAsync(
        IQueryable<T> source, 
        int page, 
        int pageSize, 
        CancellationToken cancellationToken = default)
    {
        int count = await source.CountAsync(cancellationToken);

        List<T> items = await source
           .Skip((page - 1) * pageSize)
           .Take(pageSize)
           .ToListAsync(cancellationToken);

        return new PaginationResult<T>
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = count,
            Items = items
        };
    }
}
