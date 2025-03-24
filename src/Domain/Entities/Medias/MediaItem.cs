using DTO.Enums.Media;
using System.Text.Json.Serialization;

namespace Domain.Entities.Medias;

public record MediaItem
{
    [JsonConstructor]
    public MediaItem()
    {
    }

    private MediaItem(Guid id, string name, long size, string url, bool isMain = false)
    {
        Id = id;
        Name = name;
        Size = size;
        Url = url;
        IsMain = isMain;
    }

    public Guid Id { get; init; }
    public string Name { get; set; } = null!;
    public bool IsMain { get;  set; }
    public int SortOrder { get; set; }
    public long Size { get; init; }
    public string Url { get; init; } = null!;
    public MediaItemType Type => Path.GetExtension(Url).ToMediaItemType();

    public static MediaItem Create(MediaItemKey key, string name, long size, string url, bool isMain = false) =>
         new(key.Id, name, size, url, isMain);

    public void Update(string name, bool isMain, int sortOrder)
    {
        Name = name;
        IsMain = isMain;
        SortOrder = sortOrder;
    }
    public string GetFullName()
    {
        return $"{Name}{Path.GetExtension(Url)}";
    }
}