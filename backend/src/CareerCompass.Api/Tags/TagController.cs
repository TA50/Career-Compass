using CareerCompass.Api.Common;
using CareerCompass.Api.Tags.Contracts;
using CareerCompass.Application.Tags;
using CareerCompass.Application.Tags.Commands.CreateTag;
using CareerCompass.Application.Tags.Queries.GetTagByIdQuery;
using CareerCompass.Application.Tags.Queries.GetTagsQuery;
using CareerCompass.Application.Users;
using Microsoft.AspNetCore.Mvc;

namespace CareerCompass.Api.Tags;

[ApiController]
[Route("tags")]
public class TagController(ApiControllerContext context) : ApiController(context)
{
    [HttpPost]
    public async Task<ActionResult<TagDto>> Create([FromBody] CreateTagRequest request)
    {
        var input = new CreateTagCommand(Context.UserContext.UserId, request.Name);

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

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TagDto>> Get(Guid id)
    {
        var query = new GetTagByIdQuery(Context.UserContext.UserId, id.ToString());

        var result = await Context.Sender.Send(query);

        return result.Match(
            value => Ok(Context.Mapper.Map<TagDto>(value)),
            error => error.ToProblemDetails().ToActionResult<TagDto>()
        );
    }

    [HttpGet]
    public async Task<ActionResult<IList<TagDto>>> Get()
    {
        var query = new GetTagsQuery(Context.UserContext.UserId);

        var result = await Context.Sender.Send(query);

        return result.Match(
            value => Ok(Context.Mapper.Map<IList<TagDto>>(value)),
            error => error.ToProblemDetails().ToActionResult<IList<TagDto>>()
        );
    }
}