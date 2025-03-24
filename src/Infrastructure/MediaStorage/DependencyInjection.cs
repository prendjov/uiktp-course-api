using Application.Common.MediaStorage;
using Application.Common.MediaStorage.Interfaces;
using Domain.Interfaces;
using Infrastructure.Common.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.MediaStorage;

public static class DependencyInjection
{
    public static IServiceCollection AddMediaStorage(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<MediaEntityResolver>();
        services.AddSingleton<IFileSystemProvider, FileSystemProvider>();
        services.AddSingleton<IMediaStorageHelper, MediaStorageHelper>();
        services.AddSingleton<IMediaStorageHelper, MediaStorageHelper>();
        services.AddConfigurationBoundOptions<MediaConfig>(MediaConfig.SectionName);

        services.AddScoped<IMediaEntityResolver, MediaEntityResolver>();

        var config = configuration.GetSection(MediaConfig.SectionName).Get<MediaConfig>();

        services.Configure<FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = config!.MaxFileSizeInKb * 1024;
        });

        if (config!.StorageProviderConfigs.All(cfg => cfg == null))
        {
            throw new ApplicationException("Could not resolve media storage provider");
        }

        switch (config.StorageProviderType)
        {
            case MediaStorageProviderType.FileSystem:
                RegisterFileSystemMediaStorage(services);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return services;
    }

    private static void RegisterFileSystemMediaStorage(IServiceCollection services)
    {
        services.AddSingleton<IMediaStorage, MediaFileSystemStorage>();
    }
}
