namespace Application.Common.MediaStorage.Interfaces;

public interface IMediaConfigStorageProvider
{
    MediaStorageProviderType StorageProviderType { get; }
}
