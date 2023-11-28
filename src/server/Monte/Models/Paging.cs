namespace Monte.Models;

public class Paging
{
    public const int DefaultPageSize = 10;
    
    public int Page { get; set; }
    public int PageSize { get; set; } = DefaultPageSize;

    public Paging(int page, int size)
    {
        Page = page;
        PageSize = size;
    }
    
    public Paging()
    {
    }
}
