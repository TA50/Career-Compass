using CareerCompass.Api.Common;
using CareerCompass.Api.Fields.Contracts;
using CareerCompass.Api.Tags.Contracts;
using CareerCompass.Application.Fields;
using CareerCompass.Application.Fields.Commands.CreateField;
using CareerCompass.Application.Fields.Queries.GetFieldByIdQuery;
using CareerCompass.Application.Fields.Queries.GetFieldsQuery;
using CareerCompass.Application.Tags.Commands.CreateTag;
using CareerCompass.Application.Users;
using Microsoft.AspNetCore.Mvc;

namespace CareerCompass.Api.Fields;

[ApiController]
[Route("fields")]
public class FieldController(ApiControllerContext context) : ApiController(context)
{
    [HttpPost]
    public async Task<ActionResult<FieldDto>> Create([FromBody] CreateFieldRequest request)
    {
        var input = new CreateFieldCommand(Context.UserContext.UserId,
            request.Name);

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
        var query = new GetFieldByIdQuery(Context.UserContext.UserId, id.ToString());

        var result = await Context.Sender.Send(query);

        return result.Match(
            value => Ok(Context.Mapper.Map<FieldDto>(value)),
            error => error.ToProblemDetails().ToActionResult<FieldDto>()
        );
    }

    [HttpGet]
    public async Task<ActionResult<IList<FieldDto>>> Get()
    {
        var query = new GetFieldsQuery(Context.UserContext.UserId);

        var result = await Context.Sender.Send(query);

        return result.Match(
            value => Ok(Context.Mapper.Map<IList<FieldDto>>(value)),
            error => error.ToProblemDetails().ToActionResult<IList<FieldDto>>()
        );
    }
}