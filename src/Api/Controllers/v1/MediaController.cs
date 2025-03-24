using Application.Medias.Queries;
using Application.Queries;
using DTO.Enums.Media;
using DTO.Response;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

public class MediaController : ApiControllerBase
{


    [HttpGet("{mediaEntityType}/{entityId}/{mediaItemId}/download")]
    public async Task<FileStreamResult> Download(MediaEntityType mediaEntityType, int entityId, Guid mediaItemId)
    {
        var result = await Mediator.Send(new MediaDownloadQuery(mediaEntityType, entityId, mediaItemId));

        return File(result);
    }

    [HttpGet("entity-types")]
    public async Task<IReadOnlyCollection<ListItemBaseResponse>> GetMediaEntityTypes()
    {
        return await Mediator.Send(new GetEnumValuesQuery(typeof(MediaEntityType)));
    }
}
