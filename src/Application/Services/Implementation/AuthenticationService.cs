using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Identity;
using Application.Identity;
using Domain.Entities.RefreshTokens;
using Domain.Entities.User;
using Domain.Interfaces;
using DTO.Authentication;
using DTO.Email;
using DTO.Enums.User;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;

namespace Application.Services.Implementation;

public class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IDateTime _dateTime;
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IdentityConfig _config;
    private readonly IApplicationUserManager _applicationUserManager;
    private readonly IEmailService _emailService;
    private readonly ILogger<AuthenticationService> _logger;

    public AuthenticationService(
        UserManager<ApplicationUser> userManager,
        IJwtTokenService jwtTokenService,
        IDateTime dateTime,
        IOptions<IdentityConfig> config,
        IApplicationDbContext dbContext,
        IUnitOfWork unitOfWork,
        IApplicationUserManager applicationUserManager,
        IEmailService emailService,
        ILogger<AuthenticationService> logger)
    {
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
        _dateTime = dateTime;
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
        _config = config.Value;
        _applicationUserManager = applicationUserManager;
        _emailService = emailService;
        _logger = logger;
    }

    #region Login

    public async Task<LoginResponse> Login(LoginRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.Email);

        if (user != null && await _userManager.CheckPasswordAsync(user, request.Password))
        {
            if (!await _userManager.IsEmailConfirmedAsync(user))
                throw new UnauthorizedAccessException("Unauthorized access");

            if (await _userManager.IsLockedOutAsync(user))
                throw new UnauthorizedAccessException("Unauthorized access");

            if (user.Status == UserStatus.Suspended)
                throw new UnauthorizedAccessException("Unauthorized access");

            if (user.Status != UserStatus.Active)
                throw new UnauthorizedAccessException("Unauthorized access");

            var (token, validTo, newRefreshToken) = await _jwtTokenService.CreateAsync(user);

            var refreshToken = RefreshToken.Create(newRefreshToken,
                                                   user.Id,
                                                   _dateTime.Now.Add(_config.RefreshTokenValidity));

            await _dbContext.RefreshToken.AddAsync(refreshToken);
            await _unitOfWork.SaveChangesAsync();

            await _userManager.UpdateAsync(user);

            return new LoginResponse(token, newRefreshToken);
        }

        throw new UnauthorizedAccessException("Wrong credentials");
    }

    #endregion

    #region Verify Email

    public async Task VerifyEmail(VerifyEmailRequest request)
    {
        var user = await _applicationUserManager.GetByUidAsync(request.Uid);

        if (user == null)
            throw new NotFoundException("The provided link has either expired or never existed.");

        user!.SetEmailConfirmed();

        await _unitOfWork.SaveChangesAsync();
    }

    #endregion

    #region Resend Verification Code

    public async Task ResendVerification(string email)
    {
        var user = await _userManager.FindByEmailAsync(email.ToLower());

        if (user == null)
        {
            _logger.LogInformation("User not found. Skipping resend verification email");
            return;
        }
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        user.SetEmailVerificationToken(token);
        await _userManager.UpdateAsync(user);

        byte[] tokenBytes = Encoding.UTF8.GetBytes(token);
        var tokenEncoded = WebEncoders.Base64UrlEncode(tokenBytes);

        var parameters = new Dictionary<string, string>
        {
            { "@firstName", user.FirstName },
            { "@lastName", user.LastName },
            { "@code", tokenEncoded },
            { "@uid", user.Uid.ToString() }
        };

        var htmlTemplateContent = await _emailService.GetTemplateAsync("User", "EmailVerification", parameters!);

        await _emailService.SendAsync(new MailMessageRequest(
            email.ToLower(),
            "Courses - Please verify your email",
            htmlTemplateContent,
            $"{user.FirstName} {user.LastName}"));
    }

    #endregion

    #region Refresh Token

    public async Task<LoginResponse> RefreshAccessToken(RefreshTokenRequest request)
    {
        string username = string.Empty;

        try
        {
            username = _jwtTokenService.GetClaimFromToken(request.AccessToken, "userName", false)!;
        }
        catch (Exception)
        {
            throw new UnauthorizedAccessException("Invalid accesss token");
        }

        var user = await _userManager.FindByNameAsync(username!);

        if (user == null)
            throw new UnauthorizedAccessException("Invalid accesss token");

        var currentRefreshToken = await _dbContext.RefreshToken.FirstOrDefaultAsync(t => t.Value == request.RefreshToken &&
                                                                                  t.UserId == user.Id);

        if (currentRefreshToken == null)
            throw new UnauthorizedAccessException("Invalid refresh token");

        if (currentRefreshToken.ExpiryTime <= _dateTime.Now)
            throw new UnauthorizedAccessException("Expired refresh token");

        var (accessToken, _, newRefreshToken) = await _jwtTokenService.CreateAsync(user);

        var refreshToken = RefreshToken.Create(
            newRefreshToken,
            user.Id,
            _dateTime.Now.Add(_config.RefreshTokenValidity)
            );

        _dbContext.RefreshToken.Remove(currentRefreshToken);

        if (_dbContext.Entry(currentRefreshToken) != null)
        {
            _dbContext.Entry(currentRefreshToken).State = EntityState.Deleted;
        }

        await _dbContext.RefreshToken.AddAsync(refreshToken);
        await _unitOfWork.SaveChangesAsync();

        return new LoginResponse(accessToken, newRefreshToken);
    }

    #endregion
}
