using Application.Common.Exceptions;
using Application.Common.Interfaces.Identity;
using Application.Common.Interfaces.Repository.Base;
using Domain.Entities.Base;
using Domain.Entities.Medias;
using Domain.Entities.User;
using DTO.Enums.Media;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Medias.Core;

public class MediaEntityResolver
{
    private readonly IServiceProvider _serviceProvider;

    public MediaEntityResolver(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<IWithMedia?> GetMediaEntity(int id, MediaEntityType entityType)
    {
        IWithMedia? entity = entityType switch
        {
            MediaEntityType.User => await GetUserEntity(id),
            _ => throw new NotFoundException($"Entity type {entityType} not found")
        }; ;

        return entity;
    }

    private Task<T?> GetEntity<T>(int id)
        where T : BaseEntity, IWithMedia =>
        _serviceProvider.GetRequiredService<IRepository<T>>().GetAsync(id);

    private Task<ApplicationUser?> GetUserEntity(int id)
    {
        var userManager = _serviceProvider.GetRequiredService<IApplicationUserManager>();
        return userManager.GetAsync(id);
    }
}
