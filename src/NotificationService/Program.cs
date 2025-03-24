using Application;
using Application.Common.Helpers;
using Application.Common.Interfaces;
using Infrastructure.Authentication;
using Infrastructure.Logging;
using Infrastructure.MessageBroker;
using NotificationService;
using NotificationService.Middlewares;
using NotificationService.SignalrHubs.Implementations;
using Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowAll",
        builder =>
        {
            builder
            .AllowAnyHeader()
            .AllowAnyMethod()
            .SetIsOriginAllowed((host) => true)
            .AllowCredentials();
        });
});

builder.Services.AddConsumeContextAuthProvider();
builder.Services.AddScoped<ICurrentUserService, ConsumerUserProvider>();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration, typeof(Program).Assembly);
builder.Services.AddPersistence(builder.Configuration);
builder.Services.ConfigureSignalr();

builder.Host.AddLogging();

// Add services to the container.

var app = builder.Build();
ServiceResolver.Initialize(app.Services);

// CORS
app.UseCors("AllowAll");

// Configure the HTTP request pipeline.

app.UseMiddleware<SignalrAuthMiddleware>();
app.UseAuthorization();

//MapHubs
app.MapHub<NotificationHub>("/notification");
app.Run();
