using DTO.User;
using DTO.Medias;
using Microsoft.AspNetCore.Http;
using DTO.Enums.User;

namespace Application.Services;

public interface IUserService
{
    Task ChangePassword(UserChangePasswordRequest request);
    Task ResetPassword(UserResetPasswordRequest request);
    Task ChangeStatus(UserStatus status);
    Task<UserResponse> CreateUser(UserCreateRequest request);
    Task<MediaItemResponse> UpdateProfilePicture(IFormFile picture);
    Task SuspendUser(int userId, string suspensionReason);
    Task UnsuspendUser(int userId);
    Task<UserResponse> UpdateUser(UserUpdateRequest request);
    Task ForgotPassword(string email);
    Task<UserInfoResponse> GetById(int id);
    Task<MeResponse> GetMe();
}
