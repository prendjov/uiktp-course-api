using Domain.Entities.Medias;

namespace Application.Common.MediaStorage.Interfaces;

public interface IMediaStorageHelper
{
    MediaItemKey ParseUrl(string url);
}
