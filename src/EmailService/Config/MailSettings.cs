namespace EmailService.Config;

    public class MailSettings
    {
        public const string SectionName = "MailSettings";
        public string EmailAddress { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Host { get; set; } = null!;
        public int Port { get; set; }
    }
