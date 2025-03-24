using DTO.Enums.Media;

namespace Application.Common.Services;

public interface IApiService
{
    Task<Stream?> DownloadFile(MediaEntityType mediaEntityType, int entityId, Guid mediaItemId);
}
