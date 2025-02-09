namespace Application.Interfaces.Pagination
{
    public interface IPagedResult<T>
    {
        IEnumerable<T> Items { get; }
        int TotalCount { get; }
        int PageSize { get; }
        int PageNumber { get; }
        int TotalPages { get; }
        bool HasPreviousPage { get; }
        bool HasNextPage { get; }
    }
}
