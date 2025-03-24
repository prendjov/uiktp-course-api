namespace DTO.Sorting;
public interface ISortable<out T>
    where T : Enum
{
    public T Field { get; }
    public SortOrder SortOrder { get; }
}