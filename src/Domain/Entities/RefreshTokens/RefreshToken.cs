using Domain.Entities.Base;
using Domain.Entities.User;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.RefreshTokens
{
    public class RefreshToken : IEntity
    {
        [Key]
        public string Value { get; private set; } = null!;
        public int UserId { get; private set; }
        public DateTime ExpiryTime { get; private set; }

        public virtual ApplicationUser User { get; set; } = null!;

        private RefreshToken()
        {
            
        }

        private RefreshToken(
            string value,
            int userId,
            DateTime expiryTime)
        {
            Value = value;
            UserId = userId;
            ExpiryTime = expiryTime;
        }

        public static RefreshToken Create(
            string value,
            int userId,
            DateTime expiryTime)
        {
            RefreshToken refreshToken = new(value, userId, expiryTime);

            return refreshToken;
        }
    }
}
