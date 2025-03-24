using Application.Common.Interfaces.Identity;
using Application.Identity;
using Domain.Entities.User;
using Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Immutable;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Identity.Services;

internal class JwtTokenService : IJwtTokenService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IApplicationUserManager _applicationUserManager;
    private readonly IDateTime _dateTime;
    private readonly IdentityConfig _config;

    public JwtTokenService(
        UserManager<ApplicationUser> userManager,
        IApplicationUserManager applicationUserManager,
        IOptions<IdentityConfig> config,
        IDateTime dateTime)
    {
        _userManager = userManager;
        _applicationUserManager = applicationUserManager;
        _dateTime = dateTime;
        _config = config.Value;
    }

    public async Task<(string Token, DateTimeOffset ValidTo, string RefreshToken)> CreateAsync(ApplicationUser user)
    {
        var userClaims = await _applicationUserManager.GetClaimsAsync(user);

        var roles = await _userManager.GetRolesAsync(user);
        userClaims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));

        foreach (var role in roles)
        {
            var roleClaim = new Claim(ClaimTypes.Role, role);
            userClaims.Add(roleClaim);

            var customRoleClaim = new Claim("userRole", role);
            userClaims.Add(customRoleClaim);
        }

        var token = GenerateToken(userClaims);
        var validTo = new DateTimeOffset(token.ValidTo.ToUniversalTime(), TimeSpan.Zero);
        return (new JwtSecurityTokenHandler().WriteToken(token), validTo, GenerateRefreshToken());

    }

    public string? GetClaimFromToken(string token, string claimName, bool validateLifetime) => GetPrincipalFromToken(token, validateLifetime)?.FindFirstValue(claimName);

    public ClaimsPrincipal? GetPrincipalFromToken(string token, bool validateLifetime)
    {
        if (string.IsNullOrEmpty(token)) return null;

        if (token.ToLower().StartsWith("bearer "))
            token = token.Replace("bearer ", "", StringComparison.InvariantCultureIgnoreCase);

        if (string.IsNullOrEmpty(token) || token == "null") return null;

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudience = _config.ValidAudience,
            ValidIssuer = _config.ValidIssuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.Secret)),
            RequireExpirationTime = true,
            ValidateLifetime = validateLifetime
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(
                Configuration.IdentityConstants.DefaultSecurityAlgorithm,
                StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");

        return principal;
    }

    private JwtSecurityToken GenerateToken(IEnumerable<Claim> claims)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.Secret));

        var token = new JwtSecurityToken(
            _config.ValidIssuer,
            _config.ValidAudience,
            expires: _dateTime.Now.Add(_config.TokenValidity),
            //expires: _dateTime.Now.AddMonths(-2),
            claims: claims.ToImmutableList(),
            signingCredentials: new SigningCredentials(authSigningKey, Configuration.IdentityConstants.DefaultSecurityAlgorithm)
        );

        return token;
    }


    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}
