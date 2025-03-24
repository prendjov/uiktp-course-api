using EmailService.Models;
using EmailService.Services.Interfaces;
using MassTransit;

namespace EmailService.Consumers.Users;

//public sealed class ResendVerificationEmailMessageConsumer : IConsumer<ResendVerificationEmailMessage>
//{
//    private readonly ILogger<ResendVerificationEmailMessageConsumer> _logger;
//    private readonly IEmailSender _emailSender;
//    private readonly ITemplateProvider _templateProivder;

//    public ResendVerificationEmailMessageConsumer(
//        ILogger<ResendVerificationEmailMessageConsumer> logger,
//        IEmailSender emailSender,
//        ITemplateProvider templateProivder)
//    {
//        _logger = logger;
//        _emailSender = emailSender;
//        _templateProivder = templateProivder;
//    }
//    public async Task Consume(ConsumeContext<ResendVerificationEmailMessage> context)
//    {
//        _logger.LogInformation("Sending verification message to {email}", context.Message.Email);

//        var parameters = new Dictionary<string, string>
//        {
//            { "@firstName", context.Message.FirstName },
//            { "@lastName", context.Message.LastName },
//            { "@code", context.Message.EmailVerificationCode },
//            { "@uid", context.Message.Uid.ToString() }
//        };

//        var htmlTemplateContent = await _templateProivder.GetTemplateAsync("User", "EmailVerification", parameters);

//        await _emailSender.SendAsync(new MailMessageRequest(
//            context.Message.Email,
//            "Course education - Please verify your email",
//            htmlTemplateContent,
//            $"{context.Message.FirstName} {context.Message.LastName}"));
//    }
//}
