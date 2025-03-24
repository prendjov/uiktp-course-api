using EmailService.Models;
using EmailService.Services.Interfaces;
using MassTransit;

namespace EmailService.Consumers.Users;

//public sealed class EmailConfirmedMessageConsumer : IConsumer<EmailConfirmedMessage>
//{
//    private readonly ILogger<UserCreatedMessage> _logger;
//    private readonly IEmailSender _emailSender;
//    private readonly ITemplateProvider _templateProivder;

//    public EmailConfirmedMessageConsumer(
//        ILogger<UserCreatedMessage> logger,
//        IEmailSender emailSender,
//        ITemplateProvider templateProivder)
//    {
//        _logger = logger;
//        _emailSender = emailSender;
//        _templateProivder = templateProivder;
//    }
//    public async Task Consume(ConsumeContext<EmailConfirmedMessage> context)
//    {
//        _logger.LogInformation("Sending email confirmed message to {email}", context.Message.Email);

//        var parameters = new Dictionary<string, string>
//        {
//            { "@firstName", context.Message.FirstName },
//            { "@lastName", context.Message.LastName }
//        };

//        var htmlTemplateContent = await _templateProivder.GetTemplateAsync("User", "EmailConfirmed", parameters);

//       await _emailSender.SendAsync(new MailMessageRequest(
//           context.Message.Email,
//           "Course education - Email confirmed", 
//           htmlTemplateContent,
//           $"{context.Message.FirstName} {context.Message.LastName}"));
//    }
//}
