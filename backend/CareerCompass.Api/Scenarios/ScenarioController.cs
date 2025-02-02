using CareerCompass.Api.Common;
using CareerCompass.Api.Scenarios.Contracts;
using CareerCompass.Application.Scenarios;
using CareerCompass.Application.Scenarios.Commands.CreateScenario;
using CareerCompass.Application.Tags;
using CareerCompass.Application.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareerCompass.Api.Scenarios;

[ApiController]
[Route("scenarios")]
[Authorize]
public class ScenarioController(ApiControllerContext context) : ApiController(context)
{
    [HttpPost]
    public async Task<ActionResult<ScenarioDto>> Create([FromBody] CreateScenarioDto dto)
    {
        var input = dto.ToCreateScenarioCommand(Context.UserContext.UserId);
        var result = await Context.Sender.Send(input);

        return result.Match(
            value => CreatedAtAction(
                nameof(Get),
                new { id = value.Id },
                Context.Mapper.Map<ScenarioDto>(value)
            ),
            error => error.ToProblemDetails()
                .ToActionResult<ScenarioDto>());
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