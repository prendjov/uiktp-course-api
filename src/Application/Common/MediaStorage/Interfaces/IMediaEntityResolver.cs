using Domain.Entities.Medias;
using DTO.Enums.Media;

namespace Application.Common.MediaStorage.Interfaces;

public interface IMediaEntityResolver
{
    ValueTask<IWithMedia?> GetMediaEntity(int id, MediaEntityType entityType);
}
