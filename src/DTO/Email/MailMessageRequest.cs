namespace DTO.Email;

//public class MailMessageRequest
//{
//    public string To { get; set; } = null!;
//    public string Subject { get; set; } = null!;
//    public string Body { get; set; } = null!;
//    public string ToName { get; set; } = null!;
//}

public record MailMessageRequest(
    string To,
    string Subject,
    string Body,
    string? ToName = null
    );
