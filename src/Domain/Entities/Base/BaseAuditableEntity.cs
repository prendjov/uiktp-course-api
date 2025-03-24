using Domain.Entities.Base.Interfaces;
using Domain.Entities.User;

namespace Domain.Entities.Base;

public abstract class BaseAuditableEntity : BaseEntity, IAuditableEntity
{
    public DateTime Created { get; set; }
    public int CreatedBy { get; set; }
    public ApplicationUser Creator { get; } = null!;
    public DateTime LastModified { get; set; }
    public int LastModifiedBy { get; set; }
    public ApplicationUser LastModifier { get; } = null!;
}