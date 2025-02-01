using AutoMapper;
using CareerCompass.Api.Common;
using CareerCompass.Api.Scenarios.Contracts;
using CareerCompass.Application.Scenarios;
using CareerCompass.Application.Scenarios.Commands.CreateScenario;
using CareerCompass.Application.Tags;
using CareerCompass.Application.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CareerCompass.Api.Scenarios;

[ApiController]
[Route("scenarios")]
[Authorize]
public class ScenarioController(
    ISender mediator,
    IMapper mapper,
    UserContext userContext)
    : ApiController
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateScenarioDto dto)
    {
        var input = new CreateScenarioCommand(
            UserId: userContext.UserId,
            TagIds: dto.TagIds.Select(id => new TagId(id)).ToList(),
            ScenarioFields:
            dto.ScenarioFields.Select(mapper.Map<ScenarioField>).ToList(),
            Title:
            dto.Title,
            Date:
            dto.Date
        );

        var result = await mediator.Send(input);

        return result.Match(
            value => CreatedAtAction(
                nameof(Get),
                new { id = value.Id },
                mapper.Map<ScenarioDto>(value)
            ),
            error =>
            {
                var problemDetails = MapError(error);

                return new ObjectResult(problemDetails)
                {
                    StatusCode = problemDetails.Status
                };
            }
        );
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