namespace DTO.Pagination;

public interface IPaginated
{
    int PageNumber { get; }
    int PageSize { get; }
}
