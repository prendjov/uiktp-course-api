using DTO.MessageBroker.Messages.Notification;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using NotificationService.SignalrHubs.Implementations;
using NotificationService.SignalrHubs.Interfaces;

namespace NotificationService.Consumers;

public sealed class TestNotificationMessageConsumer : IConsumer<TestNotificationMessage>
{
    private readonly IHubContext<NotificationHub, INotificationHub> _notificationHub;

    public TestNotificationMessageConsumer(IHubContext<NotificationHub, INotificationHub> notificationHub)
    {
        _notificationHub = notificationHub;
    }
    public async Task Consume(ConsumeContext<TestNotificationMessage> context)
    {
        await _notificationHub.Clients.Group($"user_{context.Message.UserId}").SendTestNotification(context.Message.Text);
    }
}
