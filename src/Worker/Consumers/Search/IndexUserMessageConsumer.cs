using Application.Features.Users.Commands;
using DTO.MessageBroker.Messages.Search;
using MassTransit;
using MediatR;

namespace Worker.Consumers.Search;

public sealed class IndexUserMessageConsumer : IConsumer<IndexUserMessage>
{
    private readonly ISender _mediatr;

    public IndexUserMessageConsumer(
        ISender mediatr)
    {
        _mediatr = mediatr;
    }

    public async Task Consume(ConsumeContext<IndexUserMessage> context)
    {
        await _mediatr.Send(new UserIndexCommand(context.Message.UserId));
    }
}
