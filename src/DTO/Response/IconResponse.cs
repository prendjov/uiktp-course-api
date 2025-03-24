namespace DTO.Response;

public sealed record IconResponse
{
    public string Image { get; init; } = null!;
    public string Color { get; init; } = null!;
    public string BackgroundColor { get; init; } = null!;
}
