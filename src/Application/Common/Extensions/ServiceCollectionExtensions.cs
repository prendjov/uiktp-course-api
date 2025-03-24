using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Application.Common.Extensions
{
    public static class ServiceCollectionExtensions
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
            return services.AddConfigurationBoundOptions<T>(configSection: typeof(T).Name);
        }
    }
}
