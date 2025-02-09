using Application.Interfaces.Pagination;

namespace Application.Common.Pagination
{
    public class PagedResult<T>(IEnumerable<T> items, int totalCount, int pageNumber, int pageSize) : IPagedResult<T>
    {
        public IEnumerable<T> Items { get; private set; } = items;
        public int TotalCount { get; private set; } = totalCount;
        public int PageSize { get; private set; } = pageSize;
        public int PageNumber { get; private set; } = pageNumber;
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }
}
