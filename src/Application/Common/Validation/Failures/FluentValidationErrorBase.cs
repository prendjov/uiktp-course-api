using System.Net;

namespace Application.Common.Validation.Failures;

public abstract class FluentValidationErrorBase : IFluentValidationError
{
    public string MessageKey => $"{GetType().Name}.Key";
    public virtual HttpStatusCode? StatusCode => null;
    public object? Data { get; init; }
}
