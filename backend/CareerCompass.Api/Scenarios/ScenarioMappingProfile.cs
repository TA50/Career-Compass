using AutoMapper;
using CareerCompass.Api.Scenarios.Contracts;
using CareerCompass.Application.Scenarios;
using CareerCompass.Application.Scenarios.Commands.CreateScenario;

namespace CareerCompass.Api.Scenarios;

public class ScenarioMappingProfile : Profile
{
    public ScenarioMappingProfile()
    {


        CreateMap<Scenario, ScenarioDto>()
            .ForMember(d => d.Id, o => o.MapFrom(src => src.Id.Value))
            .ForMember(d => d.ScenarioFields, o => o.MapFrom(src => src.ScenarioFields));

        // ScenarioField  < - >  ScenarioFieldDto

        CreateMap<ScenarioField, ScenarioFieldDto>()
            .ForMember(d => d.FieldId, o => o.MapFrom(src => src.FieldId.Value))
            .ReverseMap();
        
        // Guid  < - >  ScenarioId
        CreateMap<ScenarioId, Guid>()
            .ConvertUsing(src => src.Value);

        CreateMap<Guid, ScenarioId>()
            .ConvertUsing(src => new ScenarioId(src));


        CreateMap<string, ScenarioId>()
            .ConvertUsing(src => new ScenarioId(Guid.Parse(src)));


        CreateMap<ScenarioId, string>()
            .ConvertUsing(src => src.Value.ToString());
    }
}