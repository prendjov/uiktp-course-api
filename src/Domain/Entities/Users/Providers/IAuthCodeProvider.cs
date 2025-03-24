using Domain.Entities.User;

namespace Domain.Entities.Users.Providers;

public interface IAuthCodeProvider
{
    string GenerateEmailVerificationCodeCode();
    Task<string> GenereatePasswordResetCode(ApplicationUser user);
}
