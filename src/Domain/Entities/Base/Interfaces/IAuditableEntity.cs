using Domain.Entities.User;

namespace Domain.Entities.Base.Interfaces;
public interface IAuditableEntity
{
    DateTime Created { get; set; }
    int CreatedBy { get; set; }
    ApplicationUser Creator { get; }
    DateTime LastModified { get; set; }
    int LastModifiedBy { get; set; }
    ApplicationUser LastModifier { get; }
}