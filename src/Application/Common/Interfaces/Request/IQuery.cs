using MediatR;

namespace Application.Common.Interfaces.Request;


public interface IQuery : IRequest, IQueryBase
{
}

public interface IQuery<TResponse> : IRequest<TResponse>, IQueryBase
{
}

public interface IQueryBase
{
}
