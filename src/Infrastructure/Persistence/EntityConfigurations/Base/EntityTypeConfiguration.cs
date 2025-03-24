using Domain.Entities.Base;
using Domain.Entities.Base.Interfaces;
using Domain.Entities.Medias;
using Infrastructure.Common.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.EntityConfigurations.Base;

public abstract class EntityTypeConfigurationBase
{
    internal const int NameMaxLength = 200;
    internal const int ExternalIdMaxLength = 64;
    internal const int DescriptionMaxLength = 2048;
}

public abstract class EntityTypeConfiguration<T> : EntityTypeConfigurationBase, IEntityTypeConfiguration<T>
    where T : BaseEntity
{
    public void Configure(EntityTypeBuilder<T> builder)
    {

        if (typeof(IWithMedia).IsAssignableFrom(typeof(T)))
        {
            builder.Property(nameof(IWithMedia.Media)).HasConversion<MediaToDbJsonConverter>();
        }

        if (typeof(IAuditableEntity).IsAssignableFrom(typeof(T)))
        {
            builder.HasOne(nameof(IAuditableEntity.Creator))
                   .WithMany()
                   .HasForeignKey(nameof(IAuditableEntity.CreatedBy));


            builder.HasOne(nameof(IAuditableEntity.LastModifier))
                   .WithMany()
                   .HasForeignKey(nameof(IAuditableEntity.LastModifiedBy));


            OnConfigure(builder);
        }
    }
    protected abstract void OnConfigure(EntityTypeBuilder<T> builder);
}
