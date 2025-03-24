namespace Domain.Entities.Medias;

public record MediaUpdateData(Guid Id, bool IsMain, int SortOrder, int? DocumentTypeId = null) : IMediaUpsertData;
