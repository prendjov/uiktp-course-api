using Application.Common.Extensions;
using Application.Common.MediaStorage.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Application.Common.MediaStorage;

public class MediaFileSystemConfig : IMediaConfigStorageProvider
{
    [Required]
    public string HostUrl { get; set; } = null!;

    [Required]
    public string UploadMediaPath { get; set; } = null!;

    [Required]
    public string UploadDocumentPath { get; set; } = null!;

    public int? CacheStaticFileInHours { get; set; } = null!;
    public TimeSpan? CacheStaticFile => CacheStaticFileInHours?.Apply(hours => TimeSpan.FromHours(hours));
    public MediaStorageProviderType StorageProviderType => MediaStorageProviderType.FileSystem;
}
