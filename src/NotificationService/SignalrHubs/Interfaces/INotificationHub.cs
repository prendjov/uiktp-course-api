using DTO.Notification;

namespace NotificationService.SignalrHubs.Interfaces
{
    public interface INotificationHub
    {
        Task NewNotification(NewNotification notification);
        Task SuspendedUserAlert();
        Task SendTestNotification(string notificationText);
    }
}
