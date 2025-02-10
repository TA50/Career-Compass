using AutoMapper;
using CareerCompass.Core.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareerCompass.Api.Controllers;

public class ApiControllerContext(
    ISender sender,
    IMapper mapper)
{
    public readonly ISender Sender = sender;
    public readonly IMapper Mapper = mapper;
}

public class ApiController(ApiControllerContext context) : ControllerBase
{
    public ApiControllerContext Context { get; } = context;

    protected UserId UserId
    {
        get
        {
            // string? userId = context.UserManager.GetUserId(HttpContext.User);
            //
            // if (userId == null)
            // {
            //     if (IsAuthorizationRequired())
            //     {
            //         throw new UnauthorizedAccessException("User not found");
            //     }
            //
            //     return UserId.CreateUnique(); // Anonymous user
            // }

            return UserId.CreateUnique();
        }
    }

    private bool IsAuthorizationRequired()
    {
        // Get all the endpoint's metadata
        var endpoint = HttpContext.GetEndpoint();
        if (endpoint == null)
            return false;

        // Check for the presence of the AuthorizeFilter
        var authorize = endpoint.Metadata.GetMetadata<IAuthorizeData>();
        var allowAnonymous = endpoint.Metadata.GetMetadata<IAllowAnonymous>();

        // Authorization is required if there is an AuthorizeFilter and no AllowAnonymous
        return authorize != null && allowAnonymous == null;
    }
}