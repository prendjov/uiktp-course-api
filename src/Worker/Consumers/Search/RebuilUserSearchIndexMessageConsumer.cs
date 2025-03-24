using Application.Features.Users.Commands;
using DTO.MessageBroker.Messages.Search;
using MassTransit;
using MediatR;

namespace Worker.Consumers.Search;

public sealed class RebuilUserSearchIndexMessageConsumer : IConsumer<RebuilUserSearchIndexMessage>
{
    private readonly ISender _mediatr;
    public RebuilUserSearchIndexMessageConsumer(ISender mediatr)
    {
        _mediatr = mediatr;
    }
    public async Task Consume(ConsumeContext<RebuilUserSearchIndexMessage> context)
    {
        await _mediatr.Send(new UserRebuildSearchIndexCommand());
    }
}
