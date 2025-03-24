namespace Domain.Entities.Users;

public interface IUserUpdateData
{
    string FirstName { get; }
    string LastName { get; }
    string Email { get; }
    string? PhoneNumber { get; }
    int? CorporationId { get; }
}
