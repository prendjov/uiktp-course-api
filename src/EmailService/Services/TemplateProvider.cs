using Application.Common.Models.Configuration;
using EmailService.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace EmailService.Services;

public class TemplateProvider : ITemplateProvider
{
    private const string _templateFolder = "Templates";

    private readonly AppsConfig _appsConfig;
    public TemplateProvider(IOptions<AppsConfig> appsConfig)
    {
        _appsConfig = appsConfig.Value;
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
