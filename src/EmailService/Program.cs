using Application.Common.Helpers;
using Application.Common.Models.Configuration;
using EmailService.Config;
using EmailService.Models;
using EmailService.Services;
using EmailService.Services.Interfaces;
using Microsoft.Extensions.Options;

class Program
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
            .ConfigureServices((hostContext, services) =>
            {
                Console.WriteLine("Service running...");
                services.AddOptions<AppsConfig>().BindConfiguration(AppsConfig.SectionName);
                services.AddOptions<MailSettings>().BindConfiguration(MailSettings.SectionName);
                services.AddOptions<SendgridConfig>().BindConfiguration(SendgridConfig.SectionName);
                services.AddSingleton<ITemplateProvider, TemplateProvider>();

                var serviceProvider = services.BuildServiceProvider();

                var sendGridConfig = serviceProvider.GetRequiredService<IOptions<SendgridConfig>>();

                if (sendGridConfig?.Value?.Enabled == true)
                {
                    services.AddSingleton<IEmailSender, SendgridSender>();
                }
                else
                {
                    services.AddSingleton<IEmailSender, EmailSender>();
                }

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