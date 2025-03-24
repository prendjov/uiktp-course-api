using EmailService.Config;
using EmailService.Models;
using EmailService.Services.Interfaces;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace EmailService.Services;

public class EmailSender : IEmailSender
{
    private readonly MailSettings _mailSettings;
    private readonly ILogger<EmailSender> _logger;

    public EmailSender(
        IOptions<MailSettings> mailSettings,
        ILogger<EmailSender> logger)
    {
        _mailSettings = mailSettings.Value;
        _logger = logger;

        _logger.LogDebug("Setup EmailSender with email address {0}", _mailSettings.EmailAddress);
    }
    public async Task SendAsync(MailMessageRequest message)
    {
        var mailMessage = new MimeMessage();

        MailboxAddress from = new MailboxAddress(_mailSettings.DisplayName, _mailSettings.EmailAddress);
        mailMessage.From.Add(from);

        MailboxAddress to = new MailboxAddress(message.ToName ?? message.To, message.To);
        mailMessage.To.Add(to);

        mailMessage.Subject = message.Subject;

        BodyBuilder bodyBuilder = new BodyBuilder();
        bodyBuilder.HtmlBody = message.Body;
        bodyBuilder.TextBody = message.Body;

        mailMessage.Body = bodyBuilder.ToMessageBody();

        try
        {
            using (var smptClient = new SmtpClient())
            {
                await smptClient.ConnectAsync(_mailSettings.Host, _mailSettings.Port, true);
                await smptClient.AuthenticateAsync(_mailSettings.EmailAddress, _mailSettings.Password);
                await smptClient.SendAsync(mailMessage);
                await smptClient.DisconnectAsync(true);
                smptClient.Dispose();
            }
        }
        catch (Exception ex)
        {

            throw;
        }
    }
}
