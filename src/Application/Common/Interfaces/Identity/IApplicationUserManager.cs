using Domain.Entities.User;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Application.Common.Interfaces.Identity;

public interface IApplicationUserManager
{
    Task ValidatePassword(ApplicationUser user, string password);
    Task<ApplicationUser?> GetAsync(int userId);
    Task<IdentityResult> CreateAsync(ApplicationUser user);
    Task<IdentityResult> CreateAsync(ApplicationUser user, string password);
    Task<IdentityResult> DeleteAsync(ApplicationUser user);
    Task<IList<Claim>> GetClaimsAsync(ApplicationUser user);
    Task<ApplicationUser?> GetByUidAsync(Guid uid);
    string GenerateRandomPassword();
    Task<ApplicationUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}
