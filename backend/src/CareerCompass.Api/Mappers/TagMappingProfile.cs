using AutoMapper;
using CareerCompass.Api.Contracts.Tags;
using CareerCompass.Core.Tags;

namespace CareerCompass.Api.Mappers;

public class TagMappingProfile : Profile
{
    public TagMappingProfile()
    {
        CreateMap<Tag, TagDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));


        CreateMap<string, TagId>()
            .ConvertUsing(src => TagId.Create(Guid.Parse(src)));


        CreateMap<TagId, string>()
            .ConvertUsing(src => src.Value.ToString());
    }
}