using EmailService.Models;
using EmailService.Services.Interfaces;
using MassTransit;

namespace EmailService.Consumers.Users;

//public sealed class ForgotPasswordMessageConsumer : IConsumer<ForgotPasswordMessage>
//{
//    private readonly ILogger<UserCreatedMessage> _logger;
//    private readonly IEmailSender _emailSender;
//    private readonly ITemplateProvider _templateProivder;

//    public ForgotPasswordMessageConsumer(
//        ILogger<UserCreatedMessage> logger,
//        IEmailSender emailSender,
//        ITemplateProvider templateProivder)
//    {
//        _logger = logger;
//        _emailSender = emailSender;
//        _templateProivder = templateProivder;
//    }
//    public async Task Consume(ConsumeContext<ForgotPasswordMessage> context)
//    {
//        _logger.LogInformation("Sending forgot password message to {email}", context.Message.Email);

//        var parameters = new Dictionary<string, string>
//        {
//            { "@firstName", context.Message.FirstName },
//            { "@lastName", context.Message.LastName },
//            { "@code", context.Message.Token },
//            { "@uid", context.Message.Uid.ToString() }
//        };

//        var htmlTemplateContent = await _templateProivder.GetTemplateAsync("User", "ForgotPassword", parameters);

//        await _emailSender.SendAsync(new MailMessageRequest(
//            context.Message.Email,
//            "Course education - Reset your password",
//            htmlTemplateContent,
//            $"{context.Message.FirstName} {context.Message.LastName}"));
//    }
//}
