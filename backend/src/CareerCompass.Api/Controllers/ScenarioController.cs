using CareerCompass.Api.Contracts.Scenarios;
using CareerCompass.Api.Extensions;
using CareerCompass.Core.Common.Models;
using CareerCompass.Core.Scenarios;
using CareerCompass.Core.Scenarios.Commands.Delete;
using CareerCompass.Core.Scenarios.Queries.GetScenarioByIdQuery;
using CareerCompass.Core.Scenarios.Queries.GetScenariosQuery;
using CareerCompass.Core.Tags;
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
    public async Task<ActionResult<ScenarioDto>> Update([FromBody] UpdateScenarioRequest request)
    {
        var input = request.ToUpdateScenarioCommand(CurrentUserId);
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
        var query = new GetScenarioByIdQuery(ScenarioId.Create(id), CurrentUserId);

        var result = await Context.Sender.Send(query);

        return result.Match(
            value => Ok(Context.Mapper.Map<ScenarioDto>(value)),
            errors => errors.ToProblemDetails().ToActionResult<ScenarioDto>()
        );
    }

    [HttpGet]
    public async Task<ActionResult<PaginationResult<ScenarioDto>>> Get([FromQuery] ListScenarioQuery query)
    {
        var request = new GetScenariosQuery(
            UserId: CurrentUserId,
            TagIds: query.TagIds?.Select(TagId.Create).ToList(),
            Page: query.Page,
            PageSize: query.PageSize
        );

        var result = await Context.Sender.Send(request);

        return result.Match(
            value =>
            {
                var dto = PaginationResult<ScenarioDto>.Map(value, Context.Mapper.Map<ScenarioDto>);
                return Ok(dto);
            },
            error => error.ToProblemDetails()
                .ToActionResult<PaginationResult<ScenarioDto>>()
        );
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteScenarioCommand(ScenarioId.Create(id), CurrentUserId);

        var result = await Context.Sender.Send(command);

        if (result.IsError)
        {
            return result.ErrorsOrEmptyList.ToProblemDetails().ToActionResult();
        }

        return NoContent();
    }
}