using Application.Common.Extensions;
using Application.Common.MediaStorage;
using Application.Common.MediaStorage.Interfaces;
using Domain.Entities.Medias;
using Domain.Interfaces;
using DTO.Enums.Media;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Infrastructure.MediaStorage;

public class MediaFileSystemStorage : IMediaStorage
{
    private readonly IFileSystemProvider _fileSystemProvider;
    private readonly IMediaStorageHelper _mediaStorageHelper;
    private readonly MediaFileSystemConfig _config;

    public MediaFileSystemStorage(
        IFileSystemProvider fileSystemProvider,
        IOptions<MediaConfig> configOpt,
        IMediaStorageHelper mediaStorageHelper)
    {
        _fileSystemProvider = fileSystemProvider;
        _mediaStorageHelper = mediaStorageHelper;
        _config = configOpt.Value.FileSystem!;
    }

    public Task<Stream> Download(MediaItemKey itemKey)
    {
        var (filePath, _) = ResolvePath(itemKey);

        var fileStream = _fileSystemProvider.OpenReadFile(filePath);
        return fileStream.AsTask<Stream>();
    }

    public async Task<string> Upload(MediaItemKey itemKey, IFormFile file)
    {
        var (filePath, folderPath) = ResolvePath(itemKey);
        folderPath.EnsureDirectoryExist();

        await using var stream = _fileSystemProvider.CreateFile(filePath);
        await file.CopyToAsync(stream);

        var fileUrl = FilePathToAbsoluteUrl(filePath);
        return fileUrl;
    }

    public async Task<string> Upload(MediaItemKey itemKey, Stream file, long lenght)
    {
        var (filePath, folderPath) = ResolvePath(itemKey);
        folderPath.EnsureDirectoryExist();

        await using var stream = _fileSystemProvider.CreateFile(filePath);
        await file.CopyToAsync(stream);

        var fileUrl = FilePathToAbsoluteUrl(filePath);
        return fileUrl;
    }

    public Task<MediaItemKey> ParseUrl(string url) => _mediaStorageHelper.ParseUrl(url).AsTask();

    public Task Delete(MediaItemKey itemKey)
    {
        var (filePath, _) = ResolvePath(itemKey);
        if (_fileSystemProvider.ExistsFile(filePath))
        {
            _fileSystemProvider.DeleteFile(filePath);
        }

        return Task.CompletedTask;
    }

    private (string filePath, string folderPath) ResolvePath(MediaItemKey itemKey)
    {
        var rootPath = ResolveRootPath(itemKey.Extension.ToMediaItemType());

        var folderPath = Path.Combine(
            rootPath,
            ((int)itemKey.EntityType).ToString(),
            itemKey.EntityId.ToString()!);

        var filePath = Path.Combine(folderPath, itemKey.FileName);

        return (filePath, folderPath);
    }

    private string FilePathToAbsoluteUrl(string filePath)
    {
        // Convert file path to a file URL
        var fileUrl = filePath.Replace(@"\\", @"\").Replace(@"\", "/");

        var fileAbsoluteUrl = $"{_config.HostUrl}/{fileUrl.TrimStart('/')}";
        return fileAbsoluteUrl;
    }

    private string ResolveRootPath(MediaItemType mediaItemType) =>
        mediaItemType switch
        {
            MediaItemType.Image => _config.UploadMediaPath,
            MediaItemType.Video => _config.UploadMediaPath,
            MediaItemType.Json => _config.UploadDocumentPath,
            MediaItemType.Document => _config.UploadDocumentPath,
            MediaItemType.Archive => _config.UploadMediaPath,
            _ => throw new ArgumentOutOfRangeException("Ivalid MediaItemType")
        };
}
