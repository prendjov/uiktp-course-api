using Application.Common.Interfaces;
using Application.Common.MessageBroker;
using Domain.Entities.User;
using DTO.MessageBroker.Messages.Authenticate;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace Worker.Consumers.Authenticate;

public sealed class GenerateNewVerificationCodeMessageConsumer : IConsumer<GenerateNewVerificationCodeMessage>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ILogger<GenerateNewVerificationCodeMessageConsumer> _logger;
    private readonly IMessagePublisher _messagePublisher;
    private readonly UserManager<ApplicationUser> _userManager;

    public GenerateNewVerificationCodeMessageConsumer(
        IApplicationDbContext dbContext,
        ILogger<GenerateNewVerificationCodeMessageConsumer> logger,
        IMessagePublisher messagePublisher,
        UserManager<ApplicationUser> userManager)
    {
        _dbContext = dbContext;
        _logger = logger;
        _messagePublisher = messagePublisher;
        _userManager = userManager;
    }

    public async Task Consume(ConsumeContext<GenerateNewVerificationCodeMessage> context)
    {
        var user = await _userManager.FindByEmailAsync(context.Message.Email.ToLower());

        if (user == null)
        {
            _logger.LogInformation("User not found. Skipping resend verification email");
            return;
        }
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        user.SetEmailVerificationToken(token);
        await _userManager.UpdateAsync(user);

        byte[] tokenBytes = Encoding.UTF8.GetBytes(token);
        var tokenEncoded = WebEncoders.Base64UrlEncode(tokenBytes);

        await _messagePublisher.PublishAsync(new ResendVerificationEmailMessage
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email!,
            EmailVerificationCode = tokenEncoded,
            Uid = user.Uid
        });
    }
}
