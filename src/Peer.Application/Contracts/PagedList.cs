namespace Peer.Application.Contracts;
public class PagedList<T>
{
    public long TotalItems { get; private set; }
    public IReadOnlyList<T> Items { get; private set; }
    public long Page { get; private set; } = 1;
    public long TotalPages { get; private set; }


    public PagedList(long totalItems, long page, long totalPages, ICollection<T> items)
    {
        TotalItems = totalItems;
        Items = items.ToList().AsReadOnly();
        Page = page;
        TotalPages = totalPages;
    }
}
