using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Infrastructure.Common.Extensions;

public static class ConfigurationExtensions
{
    public static IServiceCollection AddConfigurationBoundOptions<T>(
        this IServiceCollection services,
        string configSection = "",
        string? name = null)
        where T : class
    {
        return services
            .AddOptions<T>(name ?? Options.DefaultName)
            .BindConfiguration(configSection)
            .ValidateDataAnnotations()
            .ValidateOnStart()
            .Services;
    }
    public static IServiceCollection AddConfigurationBoundOptions<T>(this IServiceCollection services)
        where T : class
    {
        return AddConfigurationBoundOptions<T>(services, configSection: typeof(T).Name);
    }
}
