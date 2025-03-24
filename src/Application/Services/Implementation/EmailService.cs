using Application.Common.Models.Configuration;
using DTO.Email;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Application.Services.Implementation;

public class EmailService : IEmailService
{
    private readonly MailSettings _mailSettings;
    private readonly ILogger<EmailService> _logger;
    private const string _templateFolder = "Templates";
    private readonly AppsConfig _appsConfig;


    public EmailService(IOptions<MailSettings> mailSettings,
        ILogger<EmailService> logger,
        IOptions<AppsConfig> appsConfig)
    {
        _mailSettings = mailSettings.Value;
        _logger = logger;
        _appsConfig = appsConfig.Value;

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
                //TODO: Set Correct SMTP Client details and uncomment the code.
                //await smptClient.AuthenticateAsync(_mailSettings.EmailAddress, _mailSettings.Password);
                //await smptClient.SendAsync(mailMessage);
                //await smptClient.DisconnectAsync(true);
                //smptClient.Dispose();
            }
        }
        catch (Exception ex)
        {

            throw;
        }
    }
    public async Task<string> GetTemplateAsync(string templateFor, string templateName)
    {
        try
        {
            string templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _templateFolder, templateFor, templateName + "Template.html");

            return await File.ReadAllTextAsync(templatePath);
        }
        catch (Exception ex)
        {

            throw;
        }
    }

    public async Task<string> GetTemplateAsync(string templateFor, string templateName, Dictionary<string, string?> parameters)
    {
        var rawHtml = await GetTemplateAsync(templateFor, templateName);
        return ParseHtmlWithParameters(rawHtml, parameters);
    }

    public string ParseHtmlWithParameters(string rawHtml, Dictionary<string, string?> parameters)
    {
        if (string.IsNullOrEmpty(rawHtml))
            throw new ArgumentNullException("Raw HTML must have value");

        if (parameters == null || !parameters.Any())
            return rawHtml;

        foreach (var parameter in parameters)
        {
            rawHtml = rawHtml.Replace("@clientAppUrl", _appsConfig.ClientUrl).Replace(parameter.Key, parameter.Value ?? "");
        }

        return rawHtml;
    }
}
