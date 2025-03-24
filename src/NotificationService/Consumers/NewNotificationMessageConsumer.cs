using AutoMapper;
using DTO.MessageBroker.Messages.Notification;
using DTO.Notification;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using NotificationService.SignalrHubs.Implementations;
using NotificationService.SignalrHubs.Interfaces;

namespace NotificationService.Consumers;

public sealed class NewNotificationMessageConsumer : IConsumer<NewNotificationMessage>
{
    private readonly IHubContext<NotificationHub, INotificationHub> _notificationHub;
    private readonly IMapper _mapper;

    public NewNotificationMessageConsumer(
        IHubContext<NotificationHub, INotificationHub> notificationHub,
        IMapper mapper)
    {
        _notificationHub = notificationHub;
        _mapper = mapper;
    }
    public async Task Consume(ConsumeContext<NewNotificationMessage> context)
    {
        var notification = _mapper.Map<NewNotification>(context.Message);

        await _notificationHub.Clients.Group($"user_{context.Message.UserId.ToString()}").NewNotification(notification);
    }
}
