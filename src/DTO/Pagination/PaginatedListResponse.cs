namespace DTO.Pagination;

public record PaginatedListResponse<T>(List<T> Items, int TotalCount);
