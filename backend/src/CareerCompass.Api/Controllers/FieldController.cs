using CareerCompass.Api.Contracts.Fields;
using CareerCompass.Api.Extensions;
using CareerCompass.Core.Fields;
using CareerCompass.Core.Fields.Commands.CreateField;
using CareerCompass.Core.Fields.Commands.DeleteField;
using CareerCompass.Core.Fields.Queries.GetFieldByIdQuery;
using CareerCompass.Core.Fields.Queries.GetFieldsQuery;
using Microsoft.AspNetCore.Mvc;

namespace CareerCompass.Api.Controllers;

[ApiController]
[Route("fields")]
public class FieldController(ApiControllerContext context) : ApiController(context)
{
    [HttpPost]
    public async Task<ActionResult<FieldDto>> Create([FromBody] CreateFieldRequest request)
    {
        var input = new CreateFieldCommand(CurrentUserId,
            request.Name,
            request.Group
        );

        var result = await Context.Sender.Send(input);

        return result.Match(
            value => CreatedAtAction(
                nameof(Get),
                new { id = value.Id },
                Context.Mapper.Map<FieldDto>(value)
            ),
            error => error.ToProblemDetails()
                .ToActionResult<FieldDto>());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<FieldDto>> Get(Guid id)
    {
        var query = new GetFieldByIdQuery(CurrentUserId, FieldId.Create(id));

        var result = await Context.Sender.Send(query);

        return result.Match(
            value => Ok(Context.Mapper.Map<FieldDto>(value)),
            error => error.ToProblemDetails().ToActionResult<FieldDto>()
        );
    }

    [HttpGet]
    public async Task<ActionResult<IList<FieldDto>>> Get()
    {
        var query = new GetFieldsQuery(CurrentUserId);

        var result = await Context.Sender.Send(query);

        return result.Match(
            value => Ok(Context.Mapper.Map<IList<FieldDto>>(value)),
            error => error.ToProblemDetails().ToActionResult<IList<FieldDto>>()
        );
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteFieldCommand(CurrentUserId, FieldId.Create(id));

        var result = await Context.Sender.Send(command);

        if (result.IsError)
        {
            return result.ErrorsOrEmptyList.ToProblemDetails().ToActionResult();
        }

        return NoContent();
    }
}