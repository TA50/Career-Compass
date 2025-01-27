using System.Collections.Immutable;
using AutoMapper;
using CareerCompass.Api.Scenarios.Contracts;
using CareerCompass.Application.Fields;
using CareerCompass.Application.Scenarios;
using CareerCompass.Application.Scenarios.UseCases.Contracts;
using CareerCompass.Application.Tags;
using CareerCompass.Application.Users;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CareerCompass.Api.Scenarios;

[ApiController]
[Route("scenarios")]
public class ScenarioController(ISender mediator, IMapper mapper)
    : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ScenarioDto>> Create([FromBody] CreateScenarioDto dto)
    {
        var input = new CreateScenarioInput(
            title: dto.Title,
            userId: new UserId(dto.UserId),
            date: dto.Date,
            tagIds: dto.TagIds.Select(i => new TagId(i)).ToImmutableList(),
            scenarioFields: dto.ScenarioFields.Select(sf => new ScenarioField(new FieldId(sf.FieldId), sf.Value))
                .ToImmutableList()
        );
        var scenario = await mediator.Send(input);

        if (scenario.IsError)
        {
            return BadRequest("Errors");
        }

        var url = Url.Action(nameof(Get), nameof(ScenarioController), new { id = scenario.Value.Id }, Request.Scheme);

        return Created(url, mapper.Map<ScenarioDto>(scenario));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ScenarioDto>> Get(Guid id)
    {
        var scenario = new Scenario(
            id: new ScenarioId(id),
            userId: UserId.NewId(),
            tagIds: new List<TagId>(),
            scenarioFields: new List<ScenarioField>(),
            title: "",
            date: DateTime.Today
        );

        return Ok(scenario);
    }
}