namespace Monte.Models;

public class PagedResponse<T>
{
    public T[] Items { get; }
    public int Page { get; }
    public int PageSize { get; }
    public int PageCount { get; }
    public int TotalCount { get; }

    public PagedResponse(T[] items, Paging paging, int totalCount)
    {
        Items = items;
        Page = paging.Page;
        PageSize = paging.PageSize;
        PageCount = (TotalCount / PageSize) + 1;
        TotalCount = totalCount;
    }
}
