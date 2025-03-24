using System.Net;

namespace Application.Common.Validation;

public interface IFluentValidationError
{
    HttpStatusCode? StatusCode { get; }
    string MessageKey { get; }
}
