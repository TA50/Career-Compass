using CareerCompass.Api.Contracts.Tags;
using CareerCompass.Api.Extensions;
using CareerCompass.Core.Tags;
using CareerCompass.Core.Tags.Commands.CreateTag;
using CareerCompass.Core.Tags.Queries.GetTagByIdQuery;
using CareerCompass.Core.Tags.Queries.GetTagsQuery;
using Microsoft.AspNetCore.Mvc;

namespace CareerCompass.Api.Controllers;

[ApiController]
[Route("tags")]
public class TagController(ApiControllerContext context) : ApiController(context)
{
    [HttpPost]
    public async Task<ActionResult<TagDto>> Create([FromBody] CreateTagRequest request)
    {
        var input = new CreateTagCommand(CurrentUserId, request.Name);

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
        var query = new GetTagByIdQuery(CurrentUserId, TagId.Create(id));

        var result = await Context.Sender.Send(query);

        return result.Match(
            value => Ok(Context.Mapper.Map<TagDto>(value)),
            error => error.ToProblemDetails().ToActionResult<TagDto>()
        );
    }

    [HttpGet]
    public async Task<ActionResult<IList<TagDto>>> Get()
    {
        var query = new GetTagsQuery(CurrentUserId);

        var result = await Context.Sender.Send(query);

        return result.Match(
            value => Ok(Context.Mapper.Map<IList<TagDto>>(value)),
            error => error.ToProblemDetails().ToActionResult<IList<TagDto>>()
        );
    }
}