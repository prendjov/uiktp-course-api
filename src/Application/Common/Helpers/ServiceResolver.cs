using Microsoft.Extensions.DependencyInjection;

namespace Application.Common.Helpers;

public static class ServiceResolver
{
    private static IServiceProvider? _currentServiceProvider;

    public static void Initialize(IServiceProvider serviceProvider)
    {
        _currentServiceProvider = serviceProvider;
    }

    public static IServiceProvider? ServiceProvider => _currentServiceProvider;

    public static T? GetService<T>()
    {
        if (_currentServiceProvider == null)
            return default;

        return _currentServiceProvider.GetService<T>();
    }

    public static T GetRequiredService<T>() where T : notnull
    {
        if (_currentServiceProvider == null)
            throw new InvalidOperationException("ServiceProvider is not initialized");

        return _currentServiceProvider.GetRequiredService<T>()
            ?? throw new InvalidOperationException($"Service {typeof(T).Name} is not registered");
    }

    public static IServiceScope CreateScope()
    {
        return _currentServiceProvider?.CreateScope()
            ?? throw new InvalidOperationException("ServiceProvider is not initialized");
    }
}
