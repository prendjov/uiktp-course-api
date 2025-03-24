using DTO.Response;

namespace DTO.User;

public class UserResponse : UserBaseResponse
{
    public string Email { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public string? SuspensionReason { get; set; }
    public DateTime DateCreated { get; set; }
    public ListItemBaseResponse Status { get; set; } = null!;
}

