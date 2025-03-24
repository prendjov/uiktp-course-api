namespace Domain.Entities.Base;
public interface IEntity
{
}

public abstract class BaseEntity : IEntity
{
    public int Id { get; set; }
}