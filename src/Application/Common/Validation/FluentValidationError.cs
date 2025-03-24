using Application.Common.Validation.Failures;

namespace Application.Common.Validation;

public abstract class FluentValidationError : FluentValidationErrorBase
{
    public string Message { get; }
    protected FluentValidationError(string message)
    {
        Message = message;
    }
}
