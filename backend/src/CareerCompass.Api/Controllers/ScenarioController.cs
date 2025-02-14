using CareerCompass.Api.Contracts.Scenarios;
using CareerCompass.Api.Extensions;
using CareerCompass.Core.Scenarios;
using CareerCompass.Core.Scenarios.Queries.GetScenariosQuery;
using CareerCompass.Core.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareerCompass.Api.Controllers;

[ApiController]
[Route("scenarios")]
[Authorize]
public class ScenarioController(ApiControllerContext context) : ApiController(context)
{
    [HttpPost]
    public async Task<ActionResult<ScenarioDto>> Create([FromBody] CreateScenarioRequest request)
    {
        var input = request.ToCreateScenarioCommand(CurrentUserId);
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


    [HttpPut]
    public async Task<ActionResult<ScenarioDto>> Update([FromBody] UpdateScenarioDto dto)
    {
        var input = dto.ToUpdateScenarioCommand(CurrentUserId);
        var result = await Context.Sender.Send(input);

        return result.Match(
            value => Ok(
                Context.Mapper.Map<ScenarioDto>(value)
            ),
            error => error.ToProblemDetails()
                .ToActionResult<ScenarioDto>());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ScenarioDto>> Get(Guid id)
    {
        var scenario = Scenario.Create(
            userId: UserId.CreateUnique(),
            title: "",
            date: DateTime.Today
        );

        return Ok(scenario);
    }

    [HttpGet]
    public async Task<ActionResult<IList<ScenarioDto>>> Get()
    {
        var query = new GetScenariosQuery(CurrentUserId);

        var result = await Context.Sender.Send(query);

        return result.Match(
            value => Ok(Context.Mapper.Map<IList<ScenarioDto>>(value)),
            error => error.ToProblemDetails()
                .ToActionResult<IList<ScenarioDto>>()
        );
    }
}