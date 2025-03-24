namespace NotificationService.Models;

public sealed class SignalrGroup
{
    public string Name { get; init; } = null!;
    public List<SignalrUser> Users { get; init; } = new List<SignalrUser>();
}
