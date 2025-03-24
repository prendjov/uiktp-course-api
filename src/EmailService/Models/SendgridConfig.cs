namespace EmailService.Models;

public class SendgridConfig
{
    public const string SectionName = "SendgridConfig";
    public string ApiKey { get; init; } = null!;
    public bool Enabled { get; set; }
}
