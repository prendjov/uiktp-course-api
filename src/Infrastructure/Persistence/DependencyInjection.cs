using Application.Common.Extensions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Repository.Base;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Configuration;
using System.Reflection;

namespace Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddConfigurationBoundOptions<DatabaseConfig>(DatabaseConfig.SectionName);

        var databaseConfig = configuration.GetSection(DatabaseConfig.SectionName).Get<DatabaseConfig>();

        if (databaseConfig!.UseInMemory)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase(databaseConfig.InMemoryDatabaseName));
        }
        else
        {
            services.AddDbContext<ApplicationDbContext>(
                options =>
                    options.UseMySql(
                        configuration.GetConnectionString("Default"),
                        new MariaDbServerVersion("10.8.3")));
        }


        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<ApplicationDbContextInitialiser>();
        services.AddRepositories();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }

    private static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepositoryRead<>), typeof(RepositoryRead<>));
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        var baseInterface = typeof(IRepository<>);
        var repositoryInterfaces = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => p.IsInterface &&
                        p.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == baseInterface));


        foreach (var iRepository in repositoryInterfaces)
        {
            var implementations =
                Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => !t.IsInterface && iRepository.IsAssignableFrom(t));

            foreach (var repository in implementations)
            {
                services.AddScoped(iRepository, repository);
            }
        }
    }
}
