using AutoMapper;
using CareerCompass.Api.Contracts.Fields;
using CareerCompass.Core.Fields;

namespace CareerCompass.Api.Mappers;

public class FieldMappingProfile : Profile
{
    public FieldMappingProfile()
    {
        CreateMap<Field, FieldDto>();


        CreateMap<string, FieldId>()
            .ConvertUsing(src => FieldId.Create(Guid.Parse(src)));


        CreateMap<FieldId, string>()
            .ConvertUsing(src => src.Value.ToString());
    }
}