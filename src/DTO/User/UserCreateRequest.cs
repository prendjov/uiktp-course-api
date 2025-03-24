using DTO.Enums.User;

namespace DTO.User;

public class UserCreateRequest
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public UserRole Role { get; set; }
}
