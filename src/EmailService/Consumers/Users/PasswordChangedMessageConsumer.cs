using EmailService.Models;
using EmailService.Services.Interfaces;
using MassTransit;

namespace EmailService.Consumers.Users;

//public sealed class PasswordChangedMessageConsumer : IConsumer<PasswordChangedMessage>
//{
//    private readonly ILogger<UserCreatedMessage> _logger;
//    private readonly IEmailSender _emailSender;
//    private readonly ITemplateProvider _templateProivder;

//    public PasswordChangedMessageConsumer(
//    ILogger<UserCreatedMessage> logger,
//    IEmailSender emailSender,
//    ITemplateProvider templateProivder)
//    {
//        _logger = logger;
//        _emailSender = emailSender;
//        _templateProivder = templateProivder;
//    }

//    public async Task Consume(ConsumeContext<PasswordChangedMessage> context)
//    {
//        _logger.LogInformation("Sending papssword change confirmed message to {email}", context.Message.Email);

//        var parameters = new Dictionary<string, string>
//        {
//            { "@firstName", context.Message.FirstName },
//            { "@lastName", context.Message.LastName }
//        };

//        var htmlTemplateContent = await _templateProivder.GetTemplateAsync("User", "PasswordChangedConfirmation", parameters);

//        await _emailSender.SendAsync(new MailMessageRequest(
//       context.Message.Email,
//       "Course education - Password Changed",
//       htmlTemplateContent,
//       $"{context.Message.FirstName} {context.Message.LastName}"));
//    }
//}
