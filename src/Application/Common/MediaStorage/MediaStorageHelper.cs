using Application.Common.Extensions;
using Application.Common.MediaStorage.Interfaces;
using Domain.Entities.Medias;
using DTO.Enums.Media;

namespace Application.Common.MediaStorage;

public sealed class MediaStorageHelper : IMediaStorageHelper
{
    public MediaItemKey ParseUrl(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) || uri.LocalPath.IsEmpty()) return MediaItemKey.Empty();

        var parts = uri.LocalPath.Split("/", StringSplitOptions.RemoveEmptyEntries).ToList();
        var indexOfId = parts.FindIndex(el => int.TryParse(el, out var _));
        if (indexOfId < 0) return MediaItemKey.Empty();

        parts = parts.Skip(indexOfId).Take(3).ToList();
        if (parts.Count != 3) return MediaItemKey.Empty();

        var (entityTypeStr, entityIdStr, fileName) = (parts[0], parts[1], parts[2]);

        if (!Enum.TryParse<MediaEntityType>(entityTypeStr, out var entityType)) return MediaItemKey.Empty();

        if (!int.TryParse(entityIdStr, out var entityId)) return MediaItemKey.Empty();

        if (!Guid.TryParse(Path.GetFileNameWithoutExtension(fileName), out var id)) return MediaItemKey.Empty();

        return MediaItemKey.NewKey(entityId, entityType, Path.GetExtension(fileName), id);
    }
}
