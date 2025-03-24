using EmailService.Models;

namespace EmailService.Services.Interfaces
{
    public interface IEmailSender
    {
        Task SendAsync(MailMessageRequest message);
    }
}