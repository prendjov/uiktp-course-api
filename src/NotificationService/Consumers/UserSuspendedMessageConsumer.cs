using AutoMapper;
using DTO.MessageBroker.Messages.Users;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using NotificationService.SignalrHubs.Implementations;
using NotificationService.SignalrHubs.Interfaces;

namespace NotificationService.Consumers;

public sealed class UserSuspendedMessageConsumer : IConsumer<UserSuspendedMessage>
{
    private readonly IHubContext<NotificationHub, INotificationHub> _notificationHub;
    private readonly IMapper _mapper;

    public UserSuspendedMessageConsumer(
        IHubContext<NotificationHub, INotificationHub> notificationHub,
        IMapper mapper)
    {
        _notificationHub = notificationHub;
        _mapper = mapper;
    }
    public async Task Consume(ConsumeContext<UserSuspendedMessage> context)
    {
        await _notificationHub.Clients.Group($"user_{context.Message.UserId.ToString()}").SuspendedUserAlert();
    }
}
