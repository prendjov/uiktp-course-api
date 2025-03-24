using MediatR;

namespace Application.Common.Interfaces.Request;

public interface ICommand: IRequest, ICommandBase
{
}

public interface ICommand<TResponse>: IRequest<TResponse>, ICommandBase
{
}

public interface ICommandBase
{
}
