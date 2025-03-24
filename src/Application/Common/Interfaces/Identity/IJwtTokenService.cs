using Domain.Entities.User;
using System.Security.Claims;

namespace Application.Identity;

public interface IJwtTokenService
{
    Task<(string Token, DateTimeOffset ValidTo, string RefreshToken)> CreateAsync(ApplicationUser user);
    ClaimsPrincipal? GetPrincipalFromToken(string token, bool validateLifetime);
    string? GetClaimFromToken(string token, string claimName, bool validateLifetime);
}
