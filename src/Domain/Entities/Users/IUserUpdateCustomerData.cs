namespace Domain.Entities.Users;

public interface IUserUpdateCustomerData
{
    string FirstName { get; }
    string LastName { get; }
    string? PhoneNumber { get; }
}
