using EmailService.Config;
using EmailService.Models;
using EmailService.Services.Interfaces;
using Microsoft.Extensions.Options;
using SendGrid;

namespace EmailService.Services;

public class SendgridSender : IEmailSender
{
    private readonly MailSettings _mailSettings;
    private readonly SendgridConfig _sendGridConfig;
    private readonly ILogger<SendgridSender> _logger;

    public SendgridSender(
        IOptions<MailSettings> mailSettings,
        IOptions<SendgridConfig> sendGridConfig,
        ILogger<SendgridSender> logger)
    {
        _mailSettings = mailSettings.Value;
        _sendGridConfig = sendGridConfig.Value;
        _logger = logger;
    }

    public async Task SendAsync(MailMessageRequest message)
    {
        var sendGridClient = new SendGridClient(_sendGridConfig.ApiKey);
        var msg = SendGrid.Helpers.Mail.MailHelper.CreateSingleEmail(
            new SendGrid.Helpers.Mail.EmailAddress(_mailSettings.EmailAddress, _mailSettings.DisplayName),
            new SendGrid.Helpers.Mail.EmailAddress(message.To, message.ToName),
            message.Subject,
            message.Body,
            message.Body);

        var response = await sendGridClient.SendEmailAsync(msg);

        _logger.LogInformation("Is Success: {success}. Sengrid response: {response}", response?.IsSuccessStatusCode == true, response);
    }
}
