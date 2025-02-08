using CareerCompass.Api.Contracts.Fields;
using CareerCompass.Api.Extensions;
using CareerCompass.Core.Fields;
using CareerCompass.Core.Fields.Commands.CreateField;
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
        var input = new CreateFieldCommand(UserId,
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
        var query = new GetFieldByIdQuery(UserId, FieldId.Create(id));

        var result = await Context.Sender.Send(query);

        return result.Match(
            value => Ok(Context.Mapper.Map<FieldDto>(value)),
            error => error.ToProblemDetails().ToActionResult<FieldDto>()
        );
    }

    [HttpGet]
    public async Task<ActionResult<IList<FieldDto>>> Get()
    {
        var query = new GetFieldsQuery(UserId);

        var result = await Context.Sender.Send(query);

        return result.Match(
            value => Ok(Context.Mapper.Map<IList<FieldDto>>(value)),
            error => error.ToProblemDetails().ToActionResult<IList<FieldDto>>()
        );
    }
}