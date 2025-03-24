namespace DTO.Sorting;

public record SortOptions<T>(T Field, SortOrder SortOrder) : ISortable<T>
    where T : Enum;
