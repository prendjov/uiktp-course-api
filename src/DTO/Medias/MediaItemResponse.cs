using DTO.Enums.Media;

namespace DTO.Medias;
public record MediaItemResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public bool IsMain { get; init; }
    public int SortOrder { get; init; }
    public int Size { get; init; }
    public string Url { get; init; } = null!;
    public MediaItemType Type { get; init; }
    public string Extension { get; init; } = null!;
}