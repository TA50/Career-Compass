using AutoMapper;
using CareerCompass.Api.Contracts.Fields;
using CareerCompass.Core.Fields;

namespace CareerCompass.Api.Mappers;

public class FieldMappingProfile : Profile
{
    public FieldMappingProfile()
    {
        CreateMap<Field, FieldDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));


        CreateMap<string, FieldId>()
            .ConvertUsing(src => FieldId.Create(Guid.Parse(src)));


        CreateMap<FieldId, string>()
            .ConvertUsing(src => src.Value.ToString());
    }
}