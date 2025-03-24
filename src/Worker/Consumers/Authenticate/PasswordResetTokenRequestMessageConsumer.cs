using Application.Common.Interfaces;
using Application.Common.MessageBroker;
using Domain.Entities.User;
using Domain.Entities.Users.Providers;
using DTO.MessageBroker.Messages.Authenticate;
using DTO.MessageBroker.Messages.Users;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using System.Web;

namespace Worker.Consumers.Authenticate
{
    public sealed class PasswordResetTokenRequestMessageConsumer : IConsumer<PasswordResetTokenRequestMessage>
    {
        private readonly IMessagePublisher _messagePublisher;
        private readonly IAuthCodeProvider _authCodeProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PasswordResetTokenRequestMessageConsumer> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public PasswordResetTokenRequestMessageConsumer(
            IMessagePublisher messagePublisher,
            IAuthCodeProvider authCodeProvider,
            IUnitOfWork unitOfWork,
            ILogger<PasswordResetTokenRequestMessageConsumer> logger,
            UserManager<ApplicationUser> userManager)
        {
            _messagePublisher = messagePublisher;
            _authCodeProvider = authCodeProvider;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _userManager = userManager;
        }
        public async Task Consume(ConsumeContext<PasswordResetTokenRequestMessage> context)
        {
            _logger.LogInformation("PasswordResetCodeRequestMessage consume");
            var user = await _userManager.FindByEmailAsync(context.Message.Email.ToLower());

            if (user == null)
            {
                _logger.LogInformation("User not found. Skip generating password reset code");
                return;
            }

            await user.GenereatePasswordResetCode(_authCodeProvider);
            await _userManager.UpdateAsync(user);

            var tokenEncoded = HttpUtility.UrlEncode(user.PasswordResetToken!);

            await _messagePublisher.PublishAsync(new ForgotPasswordMessage(
                user.FirstName,
                user.LastName,
                user.Email!,
                tokenEncoded,
                user.Uid));
            _logger.LogInformation("ForgotPasswordMessage sent from PasswordResetCodeRequestMessage consume");
        }
    }
}
