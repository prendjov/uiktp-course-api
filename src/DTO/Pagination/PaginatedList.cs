using Microsoft.EntityFrameworkCore;

namespace DTO.Pagination;

public class PaginatedList<T>
{
    public IReadOnlyCollection<T> Items { get; }
    public int PageNumber { get; }
    public int TotalPages { get; }
    public int TotalCount { get; }

    public PaginatedList(IReadOnlyCollection<T> items, int count, int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        TotalPages = count == 0 ? 0 : (int)Math.Ceiling(count / (double)pageSize);
        TotalCount = count;
        Items = items;
    }

    public bool HasPreviousPage => PageNumber > 1;

    public bool HasNextPage => PageNumber < TotalPages;

    public static PaginatedList<T> Create(IQueryable<T> source, int pageNumber, int pageSize)
    {
        var count = source.Count();
        var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToArray();

        return new PaginatedList<T>(items, count, pageNumber, pageSize);
    }

    public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize, CancellationToken cancellation = default)
    {
        var count = await source.CountAsync(cancellation);
        var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToArrayAsync(cancellation);

        return new PaginatedList<T>(items, count, pageNumber, pageSize);
    }
}
