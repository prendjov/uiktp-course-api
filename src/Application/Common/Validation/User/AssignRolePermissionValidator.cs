using Application.Common.Authorizaion;

using Application.Common.Validation;
using FluentValidation;

namespace Application.Common.Validation.User;

public sealed record AssignRolePermissionValidatorData(string CurrentUserRole, string? TargetRole);

public sealed class AssignRolePermissionValidator : BaseAbstractValidator<AssignRolePermissionValidatorData>
{
    public AssignRolePermissionValidator()
    {
        RuleFor(data => data)
            .Must(data => IsRoleAssignmentAllowed(data.CurrentUserRole, data.TargetRole))
            .WithMessage("You can't assign anyone to that role with your current role");
    }

    private static bool IsRoleAssignmentAllowed(string currentUserRole, string? targetRole)
    {
        return targetRole switch
        {
            Role.SuperAdmin => currentUserRole == Role.SuperAdmin,

            Role.Professor => currentUserRole == Role.SuperAdmin,

            Role.ProfessorHelper => currentUserRole == Role.SuperAdmin ||
                                     currentUserRole == Role.ProfessorHelper,

            Role.Student => currentUserRole == Role.SuperAdmin ||
                                     currentUserRole == Role.Professor ||
                                     currentUserRole == Role.ProfessorHelper,
            _ => false
        };
    }
}
