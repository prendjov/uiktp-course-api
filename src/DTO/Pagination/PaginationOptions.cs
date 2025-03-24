namespace DTO.Pagination;

public record PaginationOptions(int PageNumber, int PageSize) : IPaginated
{
    public static PaginationOptions TakeFirst(int count) => new(1, count);
    public static PaginationOptions NewMaxElastic() => new(1, 10000);
}
