namespace NotificationService.Models;

public sealed class SignalrUser
{
    public int UserId { get; init; }
    public string ConnectionId { get; init; } = null!;

    public SignalrUser(int userId, string connectionId)
    {
        UserId = userId;
        ConnectionId = connectionId;
    }
}
