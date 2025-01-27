using AutoMapper;
using CareerCompass.Application.Scenarios;

namespace CareerCompass.Api.Scenarios.Contracts;

public class ScenarioMappingProfile : Profile
{
    public ScenarioMappingProfile()
    {
        CreateMap<Scenario, ScenarioDto>()
            .ForMember(d => d.Id, o => o.MapFrom(src => src.Id.Value))
            .ForMember(d => d.ScenarioFields, o => o.MapFrom(src => src.ScenarioFields));


        CreateMap<ScenarioField, ScenarioFieldDto>()
            .ForMember(d => d.FieldId, o => o.MapFrom(src => src.FieldId.Value));
    }
}