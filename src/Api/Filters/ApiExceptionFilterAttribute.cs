using Application.Common.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Api.Filters;

public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
{
    private readonly IDictionary<Type, Action<ExceptionContext, bool>> _exceptionHandlers;

    public ApiExceptionFilterAttribute()
    {
        // Register known exception types and handlers.
        _exceptionHandlers = new Dictionary<Type, Action<ExceptionContext, bool>>
        {
            { typeof(ValidationException), HandleValidationException },
            { typeof(NotFoundException), HandleNotFoundException },
            { typeof(UnauthorizedAccessException), HandleUnauthorizedAccessException },
            { typeof(ForbiddenAccessException), HandleForbiddenAccessException },
        };
    }

    public override void OnException(ExceptionContext context)
    {
        HandleException(context);
        base.OnException(context);
    }

    private void HandleException(ExceptionContext context)
    {
        Type type = context.Exception.GetType();
        if (_exceptionHandlers.ContainsKey(type))
        {
            _exceptionHandlers[type].Invoke(context, false);
            return;
        }
        else
        {
            if(type == typeof(AggregateException) && context.Exception.InnerException != null)
            {
                Type innerType = context.Exception.InnerException.GetType();
                if (_exceptionHandlers.ContainsKey(innerType))
                {
                    _exceptionHandlers[innerType].Invoke(context, true);
                    return;
                }
            }
        }

        if (!context.ModelState.IsValid)
        {
            HandleInvalidModelStateException(context, false);
            return;
        }
    }

    private void HandleValidationException(ExceptionContext context, bool handleInnerException)
    {
        var exception = !handleInnerException ? (ValidationException)context.Exception : (ValidationException)context.Exception.InnerException!;

        if (exception?.Errors.Any(e => e.Value.Any(v => v.StartsWith("Access forbidden"))) == true)
        {
            HandleForbiddenAccessException(context, handleInnerException);
        }
        else
        {
            var details = new ValidationProblemDetails(exception?.Errors!)
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
            };

            context.Result = new BadRequestObjectResult(details);

            context.ExceptionHandled = true;
        }
    }

    private void HandleInvalidModelStateException(ExceptionContext context, bool handleInnerException)
    {
        var details = new ValidationProblemDetails(context.ModelState)
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
        };

        context.Result = new BadRequestObjectResult(details);

        context.ExceptionHandled = true;
    }

    private void HandleNotFoundException(ExceptionContext context, bool handleInnerException)
    {
        var exception = !handleInnerException ? (NotFoundException)context.Exception : (NotFoundException)context.Exception.InnerException!;

        var details = new ProblemDetails()
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            Title = "The specified resource was not found.",
            Detail = exception.Message
        };

        context.Result = new NotFoundObjectResult(details);

        context.ExceptionHandled = true;
    }

    private void HandleUnauthorizedAccessException(ExceptionContext context, bool handleInnerException)
    {
        var exception = !handleInnerException ? (UnauthorizedAccessException)context.Exception : (UnauthorizedAccessException)context.Exception.InnerException!;

        var details = new ProblemDetails
        {
            Status = StatusCodes.Status401Unauthorized,
            Title = "Unauthorized",
            Type = "https://tools.ietf.org/html/rfc7235#section-3.1",
            Detail = exception.Message
        };


        context.Result = new UnauthorizedObjectResult(details);

        context.ExceptionHandled = true;
    }

    private void HandleForbiddenAccessException(ExceptionContext context, bool handleInnerException)
    {
        var details = new ProblemDetails
        {
            Status = StatusCodes.Status403Forbidden,
            Title = "Forbidden",
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3"
        };

        context.Result = new ObjectResult(details)
        {
            StatusCode = StatusCodes.Status403Forbidden
        };

        context.ExceptionHandled = true;
    }
}
