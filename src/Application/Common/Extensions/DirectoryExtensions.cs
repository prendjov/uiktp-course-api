namespace Application.Common.Extensions;

public static class DirectoryExtensions
{
    public static void EnsureDirectoryExist(this string directoryPath)
    {
        if (Directory.Exists(directoryPath)) return;

        Directory.CreateDirectory(directoryPath);
    }
}
