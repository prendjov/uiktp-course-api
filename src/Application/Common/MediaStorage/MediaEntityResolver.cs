using Application.Common.Interfaces.Identity;
using Application.Common.Interfaces.Repository.Base;
using Application.Common.MediaStorage.Interfaces;
using Domain.Entities.Base;
using Domain.Entities.Medias;
using DTO.Enums.Media;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Common.MediaStorage;

public class MediaEntityResolver : IMediaEntityResolver
{
    private readonly IServiceProvider _serviceProvider;

    public MediaEntityResolver(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async ValueTask<IWithMedia?> GetMediaEntity(int id, MediaEntityType entityType)
    {
        IWithMedia? entity = entityType switch
        {
            MediaEntityType.User => await GetUserEntity(id),
            _ => throw new ArgumentOutOfRangeException()
        };

        return entity;
    }

    private async ValueTask<IWithMedia?> GetUserEntity(int id)
    {
        var _userManager = _serviceProvider.GetRequiredService<IApplicationUserManager>();
        return await _userManager.GetAsync(id);
    }

    private Task<T?> GetEntity<T>(int id)
        where T : BaseEntity, IWithMedia =>
        _serviceProvider.GetRequiredService<IRepository<T>>().GetAsync(id);
}
