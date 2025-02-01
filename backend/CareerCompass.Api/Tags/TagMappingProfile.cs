using AutoMapper;
using CareerCompass.Application.Scenarios;
using CareerCompass.Application.Tags;

namespace CareerCompass.Api.Tags;

public class TagMappingProfile : Profile
{
    public TagMappingProfile()
    {
        // Guid  < - >  TagId
        CreateMap<TagId, Guid>()
            .ConvertUsing(src => src.Value);

        CreateMap<Guid, TagId>()
            .ConvertUsing(src => new TagId(src));


        CreateMap<string, TagId>()
            .ConvertUsing(src => new TagId(Guid.Parse(src)));


        CreateMap<TagId, string>()
            .ConvertUsing(src => src.Value.ToString());
    }
}