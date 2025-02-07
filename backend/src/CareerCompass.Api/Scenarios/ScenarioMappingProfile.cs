using AutoMapper;
using CareerCompass.Api.Scenarios.Contracts;
using CareerCompass.Application.Scenarios;
using CareerCompass.Application.Scenarios.Commands.CreateScenario;

namespace CareerCompass.Api.Scenarios;

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
            .ConvertUsing(src => new ScenarioId(Guid.Parse(src)));


        CreateMap<ScenarioId, string>()
            .ConvertUsing(src => src.Value.ToString());
    }
}