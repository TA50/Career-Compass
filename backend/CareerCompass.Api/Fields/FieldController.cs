using CareerCompass.Api.Common;
using CareerCompass.Api.Fields.Contracts;
using CareerCompass.Api.Tags.Contracts;
using CareerCompass.Application.Fields;
using CareerCompass.Application.Fields.Commands.CreateField;
using CareerCompass.Application.Tags.Commands.CreateTag;
using CareerCompass.Application.Users;
using Microsoft.AspNetCore.Mvc;

namespace CareerCompass.Api.Fields;

[ApiController]
[Route("fields")]
public class FieldController(ApiControllerContext context) : ApiController(context)
{
    [HttpPost]
    public async Task<ActionResult<FieldDto>> Create([FromBody] CreateFieldDto dto)
    {
        var input = new CreateFieldCommand(Context.UserContext.UserId,
            dto.Name);

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

    [HttpGet("{id}")]
    public async Task<ActionResult<FieldDto>> Get(Guid id)
    {
        var tag = new FieldDto(
            Id: id.ToString(),
            Name: "Field Name"
        );

        return Ok(tag);
    }
}