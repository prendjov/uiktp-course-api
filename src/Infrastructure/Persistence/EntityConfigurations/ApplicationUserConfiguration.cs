using Domain.Entities.User;
using Infrastructure.Common.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.EntityConfigurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(e => e.Media)
            .HasConversion<MediaToDbJsonConverter>();

        builder.Property(e => e.FirstName)
            .HasMaxLength(20);

        builder.Property(e => e.LastName)
            .HasMaxLength(30);

        builder.Property(e => e.PhoneNumber)
            .HasMaxLength(15);

        builder.Property(e => e.PhoneNumber)
            .HasMaxLength(20);

        builder.Property(e => e.EmailVerificationToken)
            .HasMaxLength(500);

        builder.Property(e => e.PasswordResetToken)
            .HasMaxLength(500);

        builder.Property(e => e.SuspensionReason)
            .HasMaxLength(100);
    }
}