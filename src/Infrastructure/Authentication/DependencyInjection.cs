using Application.Cores.Authentication;
using Application.Features.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Infrastructure.Authentication;

public static class DependencyInjection
{
    public static IServiceCollection AddHttpContextAuthProvider(this IServiceCollection services)
    {
        services.TryAddScoped<IAuthTokenProvider, HttpContextAuthTokenProvider>();
        return services;
    }
}
