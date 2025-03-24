using AutoMapper;
using Domain.Entities.Medias;
using DTO.Medias;

namespace Application.Medias.Mappings;

public sealed class MediaMapperProfile : Profile
{
    public MediaMapperProfile()
    {
        CreateMap<Media, MediaResponse>()
            .ForMember(m => m.Items, opt => opt.MapFrom(s => s.Items));

        CreateMap<MediaItem, MediaItemResponse>()
            .ForMember(m => m.Extension, opt => opt.MapFrom(s => Path.GetExtension(s.Url)));
    }
}
