using Application.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Identity.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddCurrentUserServiceForWeb(
        this IServiceCollection services)
    {
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        return services;
    }
}
