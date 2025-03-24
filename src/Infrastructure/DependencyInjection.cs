using Application.Common.Interfaces.Identity;
using Domain.Interfaces;
using Infrastructure.Identity;
using Infrastructure.Identity.DependencyInjection;
using Infrastructure.MediaStorage;
using Infrastructure.Services;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.AddIdentityModule(configuration);
        services.AddScoped<IApplicationUserManager, ApplicationUserManager>();
        services.AddTransient<IDateTime, DateTimeService>();
        services.AddSingleton<IEncryption, EncryptionService>();
        services.AddMediaStorage(configuration);
        return services;
    }
}
