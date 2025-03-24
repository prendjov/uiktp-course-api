using Domain.Primitives;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.EntityConfigurations.Base;

public abstract class EnumerationTypeConfiguration<TEnum> : IEntityTypeConfiguration<TEnum>
    where TEnum : Enumeration<TEnum>
{
    public virtual void Configure(EntityTypeBuilder<TEnum> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasData((TEnum[])typeof(Enumeration<TEnum>).GetMethod("GetValues")!.Invoke(null, null)!);
    }
}