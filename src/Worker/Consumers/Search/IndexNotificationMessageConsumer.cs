using Application.Features.Notifications.Commands;
using DTO.MessageBroker.Messages.Search;
using MassTransit;
using MediatR;

namespace Worker.Consumers.Search;

public sealed class IndexNotificationMessageConsumer : IConsumer<IndexNotificationMessage>
{
    private readonly ISender _mediatr;
    public IndexNotificationMessageConsumer(ISender mediatr)
    {
        _mediatr = mediatr;
    }

    public async Task Consume(ConsumeContext<IndexNotificationMessage> context)
    {
        await _mediatr.Send(new NotificationIndexCommand(context.Message.NotificationId));
    }
}
