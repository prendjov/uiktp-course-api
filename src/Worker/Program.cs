using Application;
using Application.Common.Helpers;
using Application.Common.Interfaces;
using Infrastructure.ApiClient;
using Infrastructure.Authentication;
using Infrastructure.Logging;
using Infrastructure.MessageBroker;
using Infrastructure.TaskScheduler;
using Persistence;

public class Program
{
    public static async Task Main(string[] args)
    {
        var hostBuilder = new HostBuilder()
            .ConfigureAppConfiguration((hostContext, configBuilder) =>
            {
                var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                Console.WriteLine("Environment: " + environmentName);

                configBuilder.SetBasePath(Directory.GetCurrentDirectory());
                configBuilder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                configBuilder.AddJsonFile(
                    $"appsettings.{environmentName}.json",
                    optional: true, reloadOnChange: true);
                configBuilder.AddEnvironmentVariables();
            })
            .AddLogging()
            .ConfigureServices((hostContext, services) =>
            {
                Console.WriteLine("Service running...");

                services.AddTaskScheduler(typeof(Program).Assembly);
                services.AddConsumeContextAuthProvider();
                services.AddScoped<ICurrentUserService, ConsumerUserProvider>();
                services.AddInfrastructure(hostContext.Configuration, typeof(Program).Assembly);
                services.AddApiClient(hostContext.Configuration);
                services.AddApplication();
                services.AddPersistence(hostContext.Configuration);

                services.AddSingleton(sp =>
                {
                    ServiceResolver.Initialize(sp);
                    return sp;
                });

                Console.WriteLine("Added message broker...");
            });

        await hostBuilder.RunConsoleAsync();
    }
}