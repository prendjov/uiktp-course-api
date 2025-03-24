namespace Application.Common.MediaStorage.Interfaces;

public interface IFileSystemProvider
{
    Stream OpenReadFile(string filePath);
    Stream CreateFile(string filePath);
    bool ExistsFile(string filePath);
    void DeleteFile(string filePath);
}
