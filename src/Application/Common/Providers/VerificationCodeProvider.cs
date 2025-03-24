using Domain.Entities.User;
using Domain.Entities.Users.Providers;
using Microsoft.AspNetCore.Identity;
using System.Web;

namespace Application.Common.Providers;

public class VerificationCodeProvider : IAuthCodeProvider
{
    private readonly UserManager<ApplicationUser> _userManager;

    public VerificationCodeProvider(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public string GenerateEmailVerificationCodeCode()
    {
        return string.Empty;
    }

    public async Task<string> GenereatePasswordResetCode(ApplicationUser user)
    {
        var passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        return HttpUtility.UrlEncode(passwordResetToken);
    }
}
