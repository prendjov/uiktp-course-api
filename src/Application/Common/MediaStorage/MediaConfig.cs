using Application.Common.MediaStorage.Interfaces;
using ByteSizeLib;

namespace Application.Common.MediaStorage;

public class MediaConfig
{
    public const string SectionName = "Media";

    public string[] AllowedExtensions { get; set; } = Array.Empty<string>();
    public int MaxFileSizeInKb { get; set; }
    public ByteSize MaxFileSize => ByteSize.FromKiloBytes(MaxFileSizeInKb);

    public MediaFileSystemConfig? FileSystem { get; set; }

    public MediaStorageProviderType StorageProviderType => StorageProviderConfigs.First(cfg => cfg != null)!.StorageProviderType;

    public IMediaConfigStorageProvider?[] StorageProviderConfigs =>
        new IMediaConfigStorageProvider?[] { FileSystem };
}
