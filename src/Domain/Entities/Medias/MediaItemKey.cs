using DTO.Enums.Media;

namespace Domain.Entities.Medias;

public record MediaItemKey(
    Guid Id, 
    int EntityId, 
    MediaEntityType EntityType, 
    string Extension)
{
    public MediaItemKey(
        MediaItem item,
        int entityId,
        MediaEntityType entityType)
        : this(item.Id, entityId, entityType, Path.GetExtension(item.Url)) { }

    public string FileName => $"{Id.ToString().ToLower()}{Extension}";

    public bool IsEmpty => Id == Guid.Empty;

    public static MediaItemKey NewKey(int entityId, MediaEntityType entityType, string extension, Guid? id = null) => new(id ?? Guid.NewGuid(), entityId, entityType, extension);

    public static MediaItemKey Empty() => new(Guid.Empty, default!, default, default!);
}
