using Microsoft.AspNetCore.Http;

namespace DTO.User;

public class UserProfilePictureUpdateRequest
{
    public IFormFile Picture { get; set; } = null!;
}
