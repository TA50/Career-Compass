using AutoMapper;
using CareerCompass.Api.Fields.Contracts;
using CareerCompass.Application.Fields;

namespace CareerCompass.Api.Fields;

public class FieldMappingProfile : Profile
{
    public FieldMappingProfile()
    {
        CreateMap<Field, FieldDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));


        CreateMap<string, FieldId>()
            .ConvertUsing(src => new FieldId(Guid.Parse(src)));


        CreateMap<FieldId, string>()
            .ConvertUsing(src => src.Value.ToString());
    }
}