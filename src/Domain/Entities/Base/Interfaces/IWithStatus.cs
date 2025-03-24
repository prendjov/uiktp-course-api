using DTO.Enums;

namespace Domain.Entities.Base.Interfaces;

public interface IWithStatus
{
    public Status Status { get; }
    void Activate();
    void Deactivate();
}
