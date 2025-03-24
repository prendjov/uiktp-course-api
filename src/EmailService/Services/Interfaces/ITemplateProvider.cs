namespace EmailService.Services.Interfaces
{
    public interface ITemplateProvider
    {

        string ParseHtmlWithParameters(string rawHtml, Dictionary<string, string?> parameters);
        Task<string> GetTemplateAsync(string templateFor, string templateName);
        Task<string> GetTemplateAsync(string templateFor, string templateName, Dictionary<string, string?> parameters);
    }
}