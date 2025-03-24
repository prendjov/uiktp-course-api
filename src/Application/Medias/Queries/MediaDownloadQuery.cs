using Application.Common.Exceptions;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.MediaStorage;
using Application.Common.Validation;
using Domain.Interfaces;
using DTO.Enums.Media;
using DTO.Medias;
using FluentValidation;
using MimeMapping;

namespace Application.Medias.Queries;

public sealed record MediaDownloadQuery(MediaEntityType MediaEntityType, int EntityId, Guid MediaItemId) : IQuery<FileModel>;

public sealed class MediaDownloadQueryHandler : IQueryHandler<MediaDownloadQuery, FileModel>
{
    private readonly MediaEntityResolver _mediaEntityResolver;
    private readonly IMediaStorage _mediaStorage;

    public MediaDownloadQueryHandler(
        MediaEntityResolver mediaEntityResolver,
        IMediaStorage mediaStorage)
    {
        _mediaEntityResolver = mediaEntityResolver;
        _mediaStorage = mediaStorage;
    }
    public async Task<FileModel> Handle(MediaDownloadQuery query, CancellationToken cancellationToken)
    {
        var entity = await _mediaEntityResolver.GetMediaEntity(query.EntityId, query.MediaEntityType);

        if (entity == null)
            throw new NotFoundException("Not found");


        var (item, content) = await entity.Media.GetContent(query.MediaItemId, query.EntityId, _mediaStorage);

        if (content == null)
            throw new NotFoundException("Not found");

        var result = new FileModel(item.Name, content, MimeUtility.GetMimeMapping(item.Name));
        return result;
    }
}

public sealed class MediaDownloadQueryValidator : BaseAbstractValidator<MediaDownloadQuery>
{
    public MediaDownloadQueryValidator()
    {
        RuleFor(qry => qry.MediaEntityType)
            .NotEmpty()
            .IsInEnum();

        RuleFor(qry => qry.EntityId)
            .NotEmpty();

        RuleFor(qry => qry.MediaItemId)
            .NotEmpty();
    }
}