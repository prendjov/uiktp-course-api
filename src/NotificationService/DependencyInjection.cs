using Application.Identity;
using NotificationService.Auth;

namespace NotificationService;

public static class DependencyInjection
{
    public static void ConfigureSignalr(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("SignalrAuth", policy => policy.Requirements.Add(new SignalrAuthRequirement(
                                                                             services.BuildServiceProvider()
                                                                                     .GetRequiredService<IIdentityContextAccessor>(),
                                                                             services.BuildServiceProvider()
                                                                                     .GetRequiredService<IJwtTokenService>(),
                                                                             services.BuildServiceProvider()
                                                                                     .GetRequiredService<IHttpContextAccessor>())));
        });

        services.AddSignalR(options =>
        {
            options.EnableDetailedErrors = true;
        });
    }
}
