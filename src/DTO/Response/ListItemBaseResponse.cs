namespace DTO.Response;

public record ListItemBaseResponse
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;
}
