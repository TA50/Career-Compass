using CareerCompass.Application.Users.Queries.GetUser;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CareerCompass.Api.Common;

public class SetUserContextMiddleware(
    UserContext userContext,
    UserManager<IdentityUser> userManager,
    ISender mediator)
    : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var userId = userManager.GetUserId(context.User);


        if (userId != null)
        {
            var user = await mediator.Send(new GetUserByIdentityIdQuery(userId));
            if (user.IsError)
            {
                throw new UnauthorizedAccessException("User not found");
            }

            userContext.SetUserId(user.Value.Id);
        }

        await next(context);
    }
}