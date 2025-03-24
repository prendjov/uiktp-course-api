namespace Domain.Entities.Base.Interfaces;

public interface IBaseConfigurationItemData
{
    string Name { get; }
    decimal? Price { get; }
    string? Description { get; }
}
