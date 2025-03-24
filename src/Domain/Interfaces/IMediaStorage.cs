using Domain.Entities.Medias;
using Microsoft.AspNetCore.Http;

namespace Domain.Interfaces;

public interface IMediaStorage
{
    Task<Stream> Download(MediaItemKey itemKey);
    Task<string> Upload(MediaItemKey itemKey, IFormFile file);
    Task<string> Upload(MediaItemKey itemKey, Stream file, long length);
    Task<MediaItemKey> ParseUrl(string url);
    Task Delete(MediaItemKey itemKey);
}
