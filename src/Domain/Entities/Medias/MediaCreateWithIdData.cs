using Microsoft.AspNetCore.Http;

namespace Domain.Entities.Medias;

public record MediaCreateWithIdData(Guid Id, IFormFile File, bool IsMain, int SortOrder, int? DocumentTypeId = null) : IMediaUpsertData;
