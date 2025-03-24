using Domain.Interfaces;
using DTO.Enums.Media;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace Domain.Entities.Medias;

public class Media
{
    [JsonConstructor]
    public Media()
    {
    }

    public Media(MediaEntityType entityType)
    {
        EntityType = entityType;
    }

    public MediaEntityType EntityType { get; init; }

    public IReadOnlyCollection<MediaItem> Items { get; set; } = Array.Empty<MediaItem>();

    public MediaItem? GetMainOrFirstImage() =>
        GetMainImage()
        ?? Items
            .Where(i => i.Type == MediaItemType.Image)
            .MinBy(i => i.SortOrder);
    public MediaItem? GetMainImage() =>
        Items.FirstOrDefault(i => i.IsMain && i.Type == MediaItemType.Image);

    public string? GetMainImageUrl() => GetMainImage()?.Url;
    public IReadOnlyCollection<MediaItem> GetOtherImages() =>
        Items.Where(i => !i.IsMain && i.Type == MediaItemType.Image).ToList();
    public MediaItem? GetItem(Guid mediaItemId) => Items.SingleOrDefault(i => i.Id == mediaItemId);
    public async Task<(MediaItem, Stream)> GetContent(Guid itemId, int entityId, IMediaStorage mediaStorage)
    {
        var mediaItem = GetItem(itemId);
        if (mediaItem == null) return default;

        var mediaItemKey = new MediaItemKey(mediaItem.Id, entityId, EntityType, Path.GetExtension(mediaItem.Url));
        var stream = await mediaStorage.Download(mediaItemKey);
        return (mediaItem, stream);
    }
    public async Task<Media> Save(IReadOnlyCollection<IMediaUpsertData> upsertData, int entityId, IMediaStorage mediaStorage)
    {
        foreach (var data in upsertData)
        {
            await Save(data, entityId, mediaStorage);
        }

        return this;
    }
    public async Task<Media> Save(IMediaUpsertData data, int entityId, IMediaStorage mediaStorage, string? fileName = null)
    {
        var mediaItem = data switch
        {
            MediaCreateData createData => await UploadFile(createData.File, entityId, mediaStorage, null, fileName),
            MediaCreateFromStream createFromBytesData => await UploadFile(createFromBytesData.File, fileName ?? createFromBytesData.FileName, entityId, mediaStorage, createFromBytesData.Size),
            MediaCreateWithIdData createWithIdData => await UploadFile(createWithIdData.File, entityId, mediaStorage, createWithIdData.Id, fileName),
            MediaUpdateData updateData => GetItem(updateData.Id) ?? throw new InvalidOperationException($"Could not update {nameof(MediaItem)} with Id: {updateData.Id}"),
            _ => throw new ArgumentOutOfRangeException("Invalid media upsert object")
        };

        if (mediaItem != null)
            mediaItem = mediaItem with { SortOrder = data.SortOrder };

        if (data.IsMain)
        {
            Items = Items.Select(i => i with { IsMain = false }).ToArray();
        }

        if (mediaItem != null)
            mediaItem = mediaItem with { IsMain = data.IsMain };

        // Replace changed media item
        if (mediaItem != null)
            Items = Items.Where(i => i.Id != mediaItem.Id).Append(mediaItem).OrderBy(i => i.SortOrder).ToArray();

        return this;
    }
    public async Task<Media> Delete(IEnumerable<Guid> ids, int entityId, IMediaStorage mediaStorage)
    {
        foreach (var id in ids)
        {
            await Delete(id, entityId, mediaStorage);
        }

        return this;
    }
    public async Task Delete(Guid id, int entityId, IMediaStorage mediaStorage)
    {
        var mediaItemToDelete = GetItem(id);
        if (mediaItemToDelete == null) return;

        var mediaItemKey = new MediaItemKey(mediaItemToDelete.Id, entityId, EntityType, Path.GetExtension(mediaItemToDelete.Url));
        await mediaStorage.Delete(mediaItemKey);

        Items = Items.Where(i => i.Id != mediaItemToDelete.Id)
                     .Select((item, idx) => item with { SortOrder = idx + 1 })
                     .ToArray();
    }
    private async Task<MediaItem> UploadFile(IFormFile file, int entityId, IMediaStorage mediaStorage, Guid? fileId = null, string? fileName = null)
    {
        if (file != null)
        {
            var mediaKey = MediaItemKey.NewKey(entityId, EntityType, Path.GetExtension(file.FileName).ToLower(), fileId);
            var url = await mediaStorage.Upload(mediaKey, file);

            var mediaItem = MediaItem.Create(mediaKey, fileName ?? file.FileName, file.Length, url);
            return mediaItem;
        }
        return null;
    }
    private async Task<MediaItem> UploadFile(Stream file, string fileName, int entityId, IMediaStorage mediaStorage, long? size)
    {
        if (size == null)
        {
            size = file.Length;
        }

        var mediaKey = MediaItemKey.NewKey(entityId, EntityType, Path.GetExtension(fileName));
        var url = await mediaStorage.Upload(mediaKey, file, size.Value);

        var mediaItem = MediaItem.Create(mediaKey, fileName, size.Value, url);
        return mediaItem;
    }
}
