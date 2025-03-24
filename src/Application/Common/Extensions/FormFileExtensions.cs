using ByteSizeLib;
using Microsoft.AspNetCore.Http;

namespace Application.Common.Extensions;

public static class FormFileExtensions
{
    public static string GetExtension(this IFormFile file) => file.FileName
        .GetExtension()
        .ToLower();

    public static ByteSize GetSize(this IFormFile file) => ByteSize.FromBytes(file.Length);
}
