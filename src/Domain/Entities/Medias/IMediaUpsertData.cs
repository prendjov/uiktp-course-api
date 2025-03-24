namespace Domain.Entities.Medias;

public interface IMediaUpsertData
{
    bool IsMain { get; }
    int SortOrder { get; }
}
