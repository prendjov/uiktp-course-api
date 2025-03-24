using MediatR;

namespace Application.Common.Interfaces.Request.Handlers;

public interface ICommandHandler<in TCommand>: IRequestHandler<TCommand>, ICommandHandlerBase
    where TCommand : ICommand
{
}

public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>, ICommandHandlerBase
    where TCommand : ICommand<TResponse>
{
}

public interface ICommandHandlerBase
{
}

