namespace DTO.Response;

public sealed record ListItemResponse: ListItemBaseResponse
{
    public string? Code { get; init; }
}
