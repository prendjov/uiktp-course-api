using Application.Common.Validation;
using FluentValidation;

namespace Application.Common.Extensions;

public static class ValidationExtensions
{

    internal static IRuleBuilderOptions<T, TProperty> WithCustomError<T, TProperty>(
        this IRuleBuilderOptions<T, TProperty> rule,
        Func<TProperty, IFluentValidationError> errorFunc) =>
        rule.WithState((_, property) => errorFunc(property));

    internal static IRuleBuilderOptions<T, TProperty> WithCustomError<T, TProperty>(
        this IRuleBuilderOptions<T, TProperty> rule,
        Func<T, TProperty, IFluentValidationError> errorFunc) =>
        rule.WithState(errorFunc);
}
