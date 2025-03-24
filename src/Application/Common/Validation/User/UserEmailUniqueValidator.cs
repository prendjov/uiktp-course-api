using Domain.Entities.User;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace Application.Common.Validation.User;

public sealed record UserEmailUniqueValidatorData(string Email, int? UserId = null);
public sealed class UserEmailUniqueValidator : AbstractValidator<UserEmailUniqueValidatorData>
{
    public UserEmailUniqueValidator(UserManager<ApplicationUser> userManager)
    {
        RuleFor(data => data)
            .Must(
                (data, _) =>
                {
                    var user = userManager.FindByNameAsync(data.Email).Result;
                    return user == null || user.Id == data.UserId;
                })
            .WithMessage($"Your email is not unique.");
    }
}
