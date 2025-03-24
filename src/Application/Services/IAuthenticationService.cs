using DTO.Authentication;

namespace Application.Services;

public interface IAuthenticationService
{
    Task<LoginResponse> Login(LoginRequest request);
    Task VerifyEmail(VerifyEmailRequest request);
    Task ResendVerification(string email);
    Task<LoginResponse> RefreshAccessToken(RefreshTokenRequest request);
}
