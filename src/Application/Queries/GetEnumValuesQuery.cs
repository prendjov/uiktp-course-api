using Application.Common.Helpers;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using DTO.Response;

namespace Application.Queries;

public sealed record GetEnumValuesQuery(Type EnumType) : IQuery<IReadOnlyCollection<ListItemBaseResponse>>;

public sealed class GetEnumValuesQueryHandler : IQueryHandler<GetEnumValuesQuery, IReadOnlyCollection<ListItemBaseResponse>>
{
    public Task<IReadOnlyCollection<ListItemBaseResponse>> Handle(GetEnumValuesQuery query, CancellationToken cancellationToken)
    {
        var enumType = query.EnumType;

        if (!enumType.IsEnum)
            throw new InvalidOperationException("Provided type must be an enum.");

        var method = typeof(EnumHelper).GetMethod("ToListItemBaseResponses")?.MakeGenericMethod(enumType);

        if (method != null)
        {
            var result = method.Invoke(null, null) as IReadOnlyCollection<ListItemBaseResponse>;
            return Task.FromResult(result ?? throw new InvalidOperationException("Failed to invoke method"));
        }
        throw new InvalidOperationException("Failed to get method");
    }
}
