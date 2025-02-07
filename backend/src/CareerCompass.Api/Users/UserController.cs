using CareerCompass.Api.Common;
using CareerCompass.Api.Users.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace CareerCompass.Api.Users;

[ApiController]
[Route("users")]
public class UserController(ApiControllerContext context) : ApiController(context)
{
    [HttpPut]
    public async Task<ActionResult<UserDto>> Update([FromBody] UpdateUserRequest dto)
    {
        var input = dto.ToUpdateUserCommand(Context.UserContext.UserId);
        var result = await Context.Sender.Send(input);

        return result.Match(
            value => Ok(
                Context.Mapper.Map<UserDto>(value)
            ),
            error => error.ToProblemDetails()
                .ToActionResult<UserDto>());
    }
}