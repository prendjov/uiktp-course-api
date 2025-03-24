using Application.Common.MediaStorage.Interfaces;

namespace Infrastructure.MediaStorage;

public sealed class FileSystemProvider : IFileSystemProvider
{
    public Stream OpenReadFile(string filePath) => File.OpenRead(filePath);

    public Stream CreateFile(string filePath) => File.Create(filePath);
    public bool ExistsFile(string filePath) => File.Exists(filePath);
    public void DeleteFile(string filePath) => File.Delete(filePath);
}
