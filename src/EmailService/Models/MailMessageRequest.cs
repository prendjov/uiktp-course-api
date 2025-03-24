namespace EmailService.Models
{
    public record MailMessageRequest(
        string To,
        string Subject,
        string Body,
        string? ToName = null
        );
}
