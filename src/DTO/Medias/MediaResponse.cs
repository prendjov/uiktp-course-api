namespace DTO.Medias;

public record MediaResponse
{
    public IReadOnlyCollection<MediaItemResponse> Items { get; set; } = Array.Empty<MediaItemResponse>();
}
