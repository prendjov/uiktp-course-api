namespace Domain.Entities.Medias;

public record MediaCreateFromStream(Stream File, bool IsMain, int SortOrder, string FileName, long? Size = null, int? DocumentTypeId = null) 
    : IMediaUpsertData;
