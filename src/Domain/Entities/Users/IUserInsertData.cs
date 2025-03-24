using DTO.Enums.User;

namespace Domain.Entities.User;

public interface IUserInsertData
{
    string FirstName { get; }
    string LastName { get; }
    string Email { get; }
    string Password { get; }
    string? PhoneNumber { get; }
    UserRole? Role { get; }
    int? CorporationId { get; }
}
