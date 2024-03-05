namespace Peer.Application.Contracts;
public class PagedList<T>
{
    public long TotalItems { get; private set; }
    public IReadOnlyList<T> Items { get; private set; }
    public int Page { get; private set; } = 1;
    public int TotalPages { get; private set; }


    public PagedList(long totalItems, int page, int totalPages, ICollection<T> items)
    {
        TotalItems = totalItems;
        Items = items.ToList().AsReadOnly();
        Page = page;
        TotalPages = totalPages;
    }
}
