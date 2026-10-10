namespace AprilBookStore.Application.Models.Common;

public class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalItemCount { get; init; }
    public int PageCount => PageSize > 0 ? (int)Math.Ceiling((double)TotalItemCount / PageSize) : 0;
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < PageCount;
}
