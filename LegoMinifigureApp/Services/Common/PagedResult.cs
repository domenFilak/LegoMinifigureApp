namespace LegoMinifigureApp.Services.Common
{
    public sealed class PagedResult<T>
    {
        public IReadOnlyList<T> Items { get; }
        public int Page { get; }
        public int PageSize { get; }
        public int TotalCount { get; }

        public PagedResult(IReadOnlyList<T> items, int page, int pageSize, int totalCount)
        {
            Items = items;
            Page = page;
            PageSize = pageSize;
            TotalCount = totalCount;
        }

        public int TotalPages => CalculateTotalPages(TotalCount, PageSize);
        public bool HasPreviousPage => Page > 1;
        public bool HasNextPage => Page < TotalPages;

        //"Showing 21–40 of 87"
        public int FirstItemNumber => TotalCount == 0 ? 0 : (Page - 1) * PageSize + 1;
        public int LastItemNumber => (Page - 1) * PageSize + Items.Count;

        public static int CalculateTotalPages(int totalCount, int pageSize)
        {
            if (totalCount == 0)
            {
                return 1; //empty list still as page 1 of 1
            }

            return (int)Math.Ceiling(totalCount / (double)pageSize);
        }
    }
}
