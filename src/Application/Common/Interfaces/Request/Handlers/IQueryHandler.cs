using MediatR;

namespace Application.Common.Interfaces.Request.Handlers;

public interface IQueryHandler<in TQuery> : IRequestHandler<TQuery>, IQueryHandlerBase
    where TQuery : IQuery
{
}

public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse>, IQueryHandlerBase
    where TQuery : IQuery<TResponse>
{
}

public interface IQueryHandlerBase
{
}
