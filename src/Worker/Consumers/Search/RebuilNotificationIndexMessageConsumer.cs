using Application.Features.Notifications.Commands;
using DTO.MessageBroker.Messages.Search;
using MassTransit;
using MediatR;

namespace Worker.Consumers.Search;

public sealed class RebuilNotificationIndexMessageConsumer : IConsumer<RebuilNotificationIndexMessage>
{
    private readonly ISender _mediatr;
    public RebuilNotificationIndexMessageConsumer(ISender mediatr)
    {
        _mediatr = mediatr;
    }
    public async Task Consume(ConsumeContext<RebuilNotificationIndexMessage> context)
    {
        await _mediatr.Send(new NotificationRebuildSearchIndexCommand());
    }
}
