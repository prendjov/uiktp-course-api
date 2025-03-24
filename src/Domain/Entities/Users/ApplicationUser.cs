using Domain.Entities.Medias;
using Domain.Entities.RefreshTokens;
using Domain.Entities.Users;
using Domain.Entities.Users.Providers;
using Domain.Interfaces;
using DTO.Enums.Media;
using DTO.Enums.User;
using DTO.User;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities.User
{
    public class ApplicationUser : IdentityUser<int>, IWithMedia
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public DateTime? LastLoginDate { get; set; }
        public DateTime Created { get; set; }
        public UserStatus Status { get; set; }
        public Guid Uid { get; private set; }
        public Media Media { get; set; } = null!;
        public string? PasswordResetToken { get; private set; }
        public string? EmailVerificationToken { get; private set; }
        public string? SuspensionReason { get; private set; }

        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

        public static ApplicationUser Create(
            UserCreateRequest data,
            IDateTime dateTimeProvider,
            bool autoVerify = false)
        {
            var user = new ApplicationUser
            {
                FirstName = data.FirstName,
                LastName = data.LastName,
                Email = data.Email.ToLower(),
                UserName = data.Email.ToLower(),
                Created = dateTimeProvider.Now,
                Uid = Guid.NewGuid(),
                Media = new Media(MediaEntityType.User),
                PhoneNumber = data.PhoneNumber,
                Status = UserStatus.AwaitingConfirmation,
            };

            if (autoVerify)
                user.SetEmailConfirmed();

            else
                //user.AddDomainEvent(new UserCreatedEvent(user));
                user.SetEmailConfirmed();

            return user;
        }

        public void Update(UserUpdateRequest data)
        {
            FirstName = data.FirstName;
            LastName = data.LastName;
            Email = data.Email.ToLower();
            UserName = data.Email.ToLower();
            PhoneNumber = data.PhoneNumber;
        }

        public void UpdateCustomer(IUserUpdateCustomerData data)
        {
            FirstName = data.FirstName;
            LastName = data.LastName;
            PhoneNumber = data.PhoneNumber;
        }

        public void SetEmailConfirmed()
        {
            EmailConfirmed = true;
            Status = UserStatus.Active;
        }
        public void Suspend(string reason)
        {
            Status = UserStatus.Suspended;
            SuspensionReason = reason;
            //AddDomainEvent(new UserSuspendedEvent(this));
        }
        public void RemoveSuspension()
        {
            Status = UserStatus.Active;
            SuspensionReason = null;
            //AddDomainEvent(new UserSuspensionRemovedEvent(this));
        }
        public void Activate()
        {
            Status = UserStatus.Active;
            //AddDomainEvent(new UserActivatedEvent(this));
        }
        public void Deactivate()
        {
            Status = UserStatus.Deactivated;
            //AddDomainEvent(new UserDeactivatedEvent(this));
        }
        public void Delete()
        {
            Status = UserStatus.Deleted;
            //AddDomainEvent(new UserDeletedEvent(this));
        }
        public void UpdatePassword(string oldPassword, string ipAddress)
        {
            //AddDomainEvent(new PasswordChangedEvent(this, oldPassword, ipAddress));
            PasswordResetToken = null;
        }

        public async Task SetProfilePicture(IMediaUpsertData data, IMediaStorage mediaStorage)
        {
            await RemoveProfilePicture(mediaStorage);
            await Media.Save(data, Id, mediaStorage);
        }

        public async Task RemoveProfilePicture(IMediaStorage mediaStorage)
        {
            var existedPhoto = Media.GetMainOrFirstImage();

            if (existedPhoto != null)
            {
                await Media.Delete(existedPhoto.Id, Id, mediaStorage);

            }
        }

        public async Task GenereatePasswordResetCode(IAuthCodeProvider codeProvider)
        {
            PasswordResetToken = await codeProvider.GenereatePasswordResetCode(this);
        }

        public void SetEmailVerificationToken(string token)
        {
            EmailVerificationToken = token;
        }
    }
}
