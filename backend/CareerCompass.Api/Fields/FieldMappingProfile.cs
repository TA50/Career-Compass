using AutoMapper;
using CareerCompass.Application.Fields;

namespace CareerCompass.Api.Fields;

public class FieldMappingProfile : Profile
{

    public FieldMappingProfile()
    {
        
        // Guid  < - >  FieldId
        CreateMap<FieldId, Guid>()
            .ConvertUsing(src => src.Value);

        CreateMap<Guid, FieldId>()
            .ConvertUsing(src => new FieldId(src));


        CreateMap<string, FieldId>()
            .ConvertUsing(src => new FieldId(Guid.Parse(src)));


        CreateMap<FieldId, string>()
            .ConvertUsing(src => src.Value.ToString());
    }
}