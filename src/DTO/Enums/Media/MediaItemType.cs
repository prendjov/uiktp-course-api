using MimeMapping;

namespace DTO.Enums.Media;

public enum MediaItemType
{
    Image = 1,
    Video,
    Json,
    Document,
    Archive
}

public static class MediaItemTypeExtensions
{
    public static MediaItemType ToMediaItemType(this string ext)
    {
        var mimeMapping = MimeUtility.GetMimeMapping(ext);

        if (string.IsNullOrEmpty(mimeMapping)) return MediaItemType.Document;

        if (mimeMapping.Contains("json")) return MediaItemType.Json;
        if (mimeMapping.StartsWith("application/zip")) return MediaItemType.Archive;
        if (mimeMapping.StartsWith("image/")) return MediaItemType.Image;
        if (mimeMapping.StartsWith("video/")) return MediaItemType.Video;

        return MediaItemType.Document;
    }
}