namespace DTO.User;

public class UserBaseResponse
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? Picture { get; set; }
}
