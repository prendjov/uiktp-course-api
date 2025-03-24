using Microsoft.AspNetCore.Http;

namespace Domain.Entities.Medias;

public record MediaCreateData(IFormFile File, bool IsMain, int SortOrder) : IMediaUpsertData;
