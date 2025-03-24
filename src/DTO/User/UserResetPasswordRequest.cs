namespace DTO.User;

public class UserResetPasswordRequest
{
    public Guid Uid { get; set; }
    public string Token { get; set; } = null!;
    public string Password { get; set; } = null!;
}
