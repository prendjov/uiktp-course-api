namespace DTO.Response;

public sealed record ListItemWithIconResponse: ListItemBaseResponse
{
    public string? Icon { get; init; }
}
