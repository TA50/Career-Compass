using System.Security.Claims;
using AutoMapper;
using CareerCompass.Core.Users;
using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
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
    private const string UserIdClaimType = ClaimTypes.NameIdentifier;
    public ApiControllerContext Context { get; } = context;

    protected bool IsAuthenticated => HttpContext.User.Identity?.IsAuthenticated ?? false;

    protected UserId CurrentUserId
    {
        get
        {
            var principal = HttpContext.User;

            var identity = principal.Identities
                .Where(i => i.AuthenticationType == CookieAuthenticationDefaults.AuthenticationScheme)
                .FirstOrDefault(i => i.IsAuthenticated);


            var userId = identity?.FindFirst(UserIdClaimType)?.Value;
            return userId is null ? HandleAnonymousUser() : UserId.Create(userId);
        }
    }

    private UserId HandleAnonymousUser()
    {
        if (IsAuthorizationRequired())
        {
            throw new UnauthorizedAccessException("User not found");
        }

        return UserId.CreateUnique(); // Anonymous user
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

    protected ClaimsPrincipal GenerateClaimsPrincipal(UserId userId)
    {
        var claims = new List<Claim>
        {
            new(UserIdClaimType, userId.ToString())
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        return new ClaimsPrincipal(identity);
    }
}