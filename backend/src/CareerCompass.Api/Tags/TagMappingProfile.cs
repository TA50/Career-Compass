using AutoMapper;
using CareerCompass.Api.Tags.Contracts;
using CareerCompass.Application.Scenarios;
using CareerCompass.Application.Tags;

namespace CareerCompass.Api.Tags;

public class TagMappingProfile : Profile
{
    public TagMappingProfile()
    {
        CreateMap<Tag, TagDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));


        CreateMap<string, TagId>()
            .ConvertUsing(src => new TagId(Guid.Parse(src)));


        CreateMap<TagId, string>()
            .ConvertUsing(src => src.Value.ToString());
    }
}