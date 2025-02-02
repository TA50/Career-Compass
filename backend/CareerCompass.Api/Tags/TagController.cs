using CareerCompass.Api.Common;
using CareerCompass.Api.Tags.Contracts;
using CareerCompass.Application.Tags;
using CareerCompass.Application.Tags.Commands.CreateTag;
using CareerCompass.Application.Users;
using Microsoft.AspNetCore.Mvc;

namespace CareerCompass.Api.Tags;

[ApiController]
[Route("tags")]
public class TagController(ApiControllerContext context) : ApiController(context)
{
    [HttpPost]
    public async Task<ActionResult<TagDto>> Create([FromBody] CreateTagDto dto)
    {
        var input = new CreateTagCommand(Context.UserContext.UserId, dto.Name);

        var result = await Context.Sender.Send(input);

        return result.Match(
            value => CreatedAtAction(
                nameof(Get),
                new { id = value.Id },
                Context.Mapper.Map<TagDto>(value)
            ),
            error => error.ToProblemDetails()
                .ToActionResult<TagDto>());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TagDto>> Get(Guid id)
    {
        var tag = new Tag(
            id: new TagId(id),
            userId: UserId.NewId(),
            name: "Tag Name"
        );

        return Ok(tag);
    }
}