using AutoMapper;
using CareerCompass.Api.Contracts.Scenarios;
using CareerCompass.Core.Scenarios;

namespace CareerCompass.Api.Mappers;

public class ScenarioMappingProfile : Profile
{
    public ScenarioMappingProfile()
    {
        CreateMap<Scenario, ScenarioDto>();
        // ScenarioField  < - >  ScenarioFieldDto

        CreateMap<ScenarioField, ScenarioFieldDto>()
            .ForMember(d => d.FieldId, o => o.MapFrom(src => src.FieldId.Value))
            .ReverseMap();

        CreateMap<string, ScenarioId>()
            .ConvertUsing(src => ScenarioId.Create(Guid.Parse(src)));


        CreateMap<ScenarioId, string>()
            .ConvertUsing(src => src.Value.ToString());
    }
}