using Application.Common.Authorizaion;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Identity;
using Application.Common.Validation.User;
using AutoMapper;
using Domain.Entities.Medias;
using Domain.Entities.User;
using Domain.Entities.Users.Providers;
using Domain.Interfaces;
using DTO.Email;
using DTO.Enums.User;
using DTO.Medias;
using DTO.User;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Text;
using System.Web;

namespace Application.Services.Implementation;

public class UserService : IUserService
{
    private readonly ICurrentUserService _currentUserService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTime _dateTimeProvider;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IApplicationUserManager _applicationUserManager;
    private readonly IMediaStorage _mediaStorage;
    private readonly IMapper _mapper;
    private readonly AssignRolePermissionValidator _roleValidator;
    private readonly ILogger<UserService> _logger;
    private readonly IAuthCodeProvider _authCodeProvider;
    private readonly IEmailService _emailService;
    private readonly UserEmailUniqueValidator _uniqueEmailValidator;


    public UserService(ICurrentUserService currentUserService,
                       UserManager<ApplicationUser> userManager,
                       IApplicationDbContext dbContext,
                       IUnitOfWork unitOfWork,
                       IDateTime dateTimeProvider,
                       IHttpContextAccessor httpContextAccessor,
                       IApplicationUserManager applicationUserManager,
                       IMediaStorage mediaStorage,
                       IMapper mapper,
                       AssignRolePermissionValidator roleValidator,
                       ILogger<UserService> logger,
                       IAuthCodeProvider authCodeProvider,
                       IEmailService emailService,
                       UserEmailUniqueValidator uniqueEmailValidator)
    {
        _currentUserService = currentUserService;
        _userManager = userManager;
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _dateTimeProvider = dateTimeProvider;
        _httpContextAccessor = httpContextAccessor;
        _mediaStorage = mediaStorage;
        _applicationUserManager = applicationUserManager;
        _roleValidator = roleValidator;
        _logger = logger;
        _authCodeProvider = authCodeProvider;
        _emailService = emailService;
        _uniqueEmailValidator = uniqueEmailValidator;
    }

    #region Get By Id

    public async Task<UserInfoResponse> GetById(int id)
    {
        var user = await _dbContext.User.FirstOrDefaultAsync(d => d.Id == id);

        if (user == null)
            throw new NotFoundException("User was not found");


        return _mapper.Map<UserInfoResponse>(user);
    }

    #endregion

    #region Get Me

    public async Task<MeResponse> GetMe()
    {
        var userId = _currentUserService.UserId;

        if(userId == null)
            throw new NotFoundException("Logged user can't be found");

        var user = await _dbContext.User.FirstOrDefaultAsync(d => d.Id == userId);

        if (user == null)
            throw new NotFoundException("User was not found");

        return _mapper.Map<MeResponse>(user);
    }

    #endregion

    #region Change Password

    public async Task ChangePassword(UserChangePasswordRequest request)
    {
        var user = await _userManager.FindByIdAsync(_currentUserService.UserId!.ToString()!);

        if (user == null)
            throw new UnauthorizedAccessException();
        var result = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);

        if (!result.Succeeded)
        {
            throw new ApplicationException("Password update failed");
        }

        // TODO: Store password change in database
        var oldPassword = _userManager.PasswordHasher.HashPassword(user, request.OldPassword);
        var ipAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();

        user.UpdatePassword(oldPassword, ipAddress);
        await _unitOfWork.SaveChangesAsync();

    }

    #endregion

    #region Reset Password

    public async Task ResetPassword(UserResetPasswordRequest request)
    {
        var user = await _applicationUserManager.GetByUidAsync(request.Uid);

        if (user == null)
            throw new NotFoundException("User not found");

        if (string.IsNullOrEmpty(user.PasswordResetToken))
            throw new ApplicationException("Invalid token");

        await _applicationUserManager.ValidatePassword(user, request.Password);

        string oldPassword = user.PasswordHash!;

        var tokenDecoded = HttpUtility.UrlDecode(request.Token);

        if (tokenDecoded != user.PasswordResetToken)
            throw new ApplicationException("Invalid token");

        var removePasswordResult = await _userManager.RemovePasswordAsync(user);

        if (!removePasswordResult.Succeeded)
            throw new ApplicationException("Password update failed");

        var addPasswordResult = await _userManager.AddPasswordAsync(user, request.Password);

        if (!addPasswordResult.Succeeded)
        {
            throw new ApplicationException("Password update failed");
        }

        // TODO: Store password change in database
        var ipAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();

        user.UpdatePassword(oldPassword, ipAddress);
        await _unitOfWork.SaveChangesAsync();
    }

    #endregion

    #region Change Status

    public async Task ChangeStatus(UserStatus status)
    {
        var user = await _dbContext.User
            .FirstOrDefaultAsync(u => u.Id == _currentUserService.UserId);

        if (user == null)
            throw new NotFoundException("User was not found");

        switch (status)
        {
            case UserStatus.Active:
                user.Activate();
                break;
            case UserStatus.Deactivated:
                user.Deactivate();
                break;
        }

        _dbContext.User.Update(user);
        await _unitOfWork.SaveChangesAsync();
    }

    #endregion

    #region Create User

    public async Task<UserResponse> CreateUser(UserCreateRequest request)
    {
        var userRole = await ConfirmRole(request.Role);

        _uniqueEmailValidator.ValidateAndThrow(new UserEmailUniqueValidatorData(request.Email));

        var user = ApplicationUser.Create(
            request,
            _dateTimeProvider);

        await _applicationUserManager.CreateAsync(user, request.Password);
        await _userManager.AddClaimAsync(user, new Claim("scope", "default"));
        await _userManager.AddToRoleAsync(user, userRole);

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        user.SetEmailVerificationToken(token);

        await _unitOfWork.SaveChangesAsync();

        var userToMail = await _userManager.FindByEmailAsync(request.Email.ToLower());


        byte[] tokenBytes = Encoding.UTF8.GetBytes(token);
        var tokenEncoded = WebEncoders.Base64UrlEncode(tokenBytes);

        if (userToMail != null)
            await SendEmail(userToMail.Email!,
                            userToMail.FirstName,
                            userToMail.LastName,
                            tokenEncoded,
                            userToMail.Uid.ToString());


        return _mapper.Map<UserResponse>(user);
    }

    private async Task SendEmail(string email, string firstName, string lastName, string verificationCode, string uid)
    {
        var parameters = new Dictionary<string, string>
        {
            { "@firstName", firstName },
            { "@lastName", lastName },
            { "@code", verificationCode},
            { "@uid", uid }
        };

        var htmlTemplateContent = await _emailService.GetTemplateAsync("User", "EmailVerification", parameters!);

        await _emailService.SendAsync(new MailMessageRequest(
            email,
            "Courses - Please verify your email",
            htmlTemplateContent,
            $"{firstName} {lastName}"));

    }

    private async Task<string> ConfirmRole(UserRole? roleEnum)
    {
        if (_currentUserService.UserId != null)
        {
            var currentUser = await _userManager.FindByIdAsync(_currentUserService.UserId!.ToString()!);

            var currentUserRole = (await _userManager.GetRolesAsync(currentUser!)).First();

            var role = ConvertRoleToString(roleEnum);

            _roleValidator.ValidateAndThrow(new AssignRolePermissionValidatorData(currentUserRole, role));

            return role ?? Role.Student;
        }

        else return Role.Student;
    }

    private string ConvertRoleToString(UserRole? role)
    {
        switch (role)
        {
            case UserRole.SuperAdmin:
                return Role.SuperAdmin;
            case UserRole.Professor:
                return Role.Professor;
            case UserRole.ProfessorHelper:
                return Role.ProfessorHelper;
            case UserRole.Student:
                return Role.Student;
            default:
                return Role.Student;
        }
    }

    #endregion

    #region Update Profile Picture

    public async Task<MediaItemResponse> UpdateProfilePicture(IFormFile picture)
    {
        var user = await _applicationUserManager.GetAsync((int)_currentUserService.UserId!);

        if (user == null)
            throw new NotFoundException("User was not found");


        var existedPhoto = user.Media.GetMainOrFirstImage();
        var mediaCreateData = new MediaCreateData(picture, true, 1);
        await user.SetProfilePicture(mediaCreateData, _mediaStorage);

        if (existedPhoto != null)
        {
            await user.Media.Delete(existedPhoto.Id, user.Id, _mediaStorage);
        }

        await _userManager.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<MediaItemResponse>(user.Media.GetMainOrFirstImage());
    }

    #endregion

    #region Suspend User

    public async Task SuspendUser(int userId, string suspensionReason)
    {
        var user = await _dbContext.User.FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            throw new NotFoundException("User was not found");

        user.Suspend(suspensionReason);

        _dbContext.User.Update(user);
        await _unitOfWork.SaveChangesAsync();
    }

    #endregion

    #region Unsuspend User

    public async Task UnsuspendUser(int userId)
    {
        var user = await _dbContext.User.FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            throw new NotFoundException("User was not found");

        user.RemoveSuspension();

        _dbContext.User.Update(user);
        await _unitOfWork.SaveChangesAsync();
    }

    #endregion

    #region Update User

    public async Task<UserResponse> UpdateUser(UserUpdateRequest request)
    {
        var user = await _applicationUserManager.GetAsync((int)_currentUserService.UserId!);

        if (user == null)
            throw new NotFoundException("User was not found");

        user.Update(request);

        await _userManager.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<UserResponse>(user);
    }

    #endregion

    #region Forgot Password

    public async Task ForgotPassword(string email)
    {
        var user = await _userManager.FindByEmailAsync(email.ToLower());

        if (user == null)
        {
            _logger.LogInformation("User not found. Skip generating password reset code");
            return;
        }

        await user.GenereatePasswordResetCode(_authCodeProvider);
        await _userManager.UpdateAsync(user);

        var tokenEncoded = HttpUtility.UrlEncode(user.PasswordResetToken!);

        _logger.LogInformation("Sending forgot password message to {email}", email);

        var parameters = new Dictionary<string, string>
        {
            { "@firstName", user.FirstName },
            { "@lastName", user.LastName },
            { "@code", tokenEncoded },
            { "@uid", user.Uid.ToString() }
        };

        var htmlTemplateContent = await _emailService.GetTemplateAsync("User", "ForgotPassword", parameters!);

        await _emailService.SendAsync(new MailMessageRequest(email,
            "Courses - Reset your password",
            htmlTemplateContent,
            $"{user.FirstName} {user.LastName}"));
    }

    #endregion

}
