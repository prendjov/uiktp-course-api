using DTO.Email;

namespace Application.Services;

public interface IEmailService
{
    Task SendAsync(MailMessageRequest message);
    string ParseHtmlWithParameters(string rawHtml, Dictionary<string, string?> parameters);
    Task<string> GetTemplateAsync(string templateFor, string templateName);
    Task<string> GetTemplateAsync(string templateFor, string templateName, Dictionary<string, string?> parameters);
}
