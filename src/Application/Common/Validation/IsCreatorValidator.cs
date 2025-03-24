using Application.Common.Interfaces;
using Domain.Entities.Base;
using FluentValidation;

namespace Application.Common.Validation;

public sealed class IsCreatorValidator : BaseAbstractValidator<BaseAuditableEntity>
{
    public IsCreatorValidator(ICurrentUserService currentUserService)
    {
        RuleFor(entity => entity)
            .Must((entity) =>
            {
                return entity.CreatedBy == currentUserService.UserId;
            })
            .WithErrorCode("FORBIDDEN ACCESS")
            .WithMessage(entity => $"Access forbidden for entity with ID = {entity.Id}");
    }

    public async Task ValidateAndThrowForbiddenAsync(BaseAuditableEntity entity, CancellationToken cancellationToken)
    {
        var validationResult = await ValidateAsync(entity, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new Common.Exceptions.ValidationException(validationResult.Errors);
        }
    }
}