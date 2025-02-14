using CareerCompass.Api.Contracts.Users;
using CareerCompass.Api.Extensions;
using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Common.Abstractions.Email;
using CareerCompass.Core.Users;
using CareerCompass.Core.Users.Commands.ConfirmEmail;
using CareerCompass.Core.Users.Commands.Register;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CareerCompass.Api.Controllers;

[ApiController]
[Route("users")]
public class UserController(
    ApiControllerContext context,
    ILoggerAdapter<UserController> logger,
    IEmailSender emailSender,
    IUrlHelperFactory urlHelperFactory,
    IConfiguration config)
    : ApiController(context)
{
    [HttpPut]
    public async Task<ActionResult<UserDto>> Update([FromBody] UpdateUserRequest dto)
    {
        var input = dto.ToUpdateUserCommand(CurrentUserId);
        var result = await Context.Sender.Send(input);

        return result.Match(
            value => Ok(
                Context.Mapper.Map<UserDto>(value)
            ),
            error => error.ToProblemDetails()
                .ToActionResult<UserDto>());
    }


    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<RegisterResponse>> Register([FromBody] RegisterRequest dto,
        string returnUrl,
        CancellationToken cancellationToken)
    {
        var input = dto.ToRegisterCommand();

        var result = await Context.Sender.Send(input, cancellationToken);
        if (result.IsError)
        {
            return result.Errors.ToProblemDetails()
                .ToActionResult<RegisterResponse>();
        }


        // Send email
        return await SendEmail(result.Value, returnUrl, cancellationToken);
    }

    private async Task<ActionResult<RegisterResponse>> SendEmail(RegisterCommandResult result, string returnUrl,
        CancellationToken cancellationToken)
    {
        var from = config["RegistrationSender"];
        if (string.IsNullOrEmpty(from))
        {
            logger.LogError("RegistrationSender is not configured");

            return Problem("Internal error", statusCode: StatusCodes.Status500InternalServerError);
        }


        var confirmUrl = GenerateConfirmationUrl(result, returnUrl);

        Console.WriteLine(confirmUrl);

        if (string.IsNullOrEmpty(confirmUrl))
        {
            logger.LogError("Failed to generate confirmation URL for user with id {UserId}", result.UserId);
            return Problem("Internal error", statusCode: StatusCodes.Status500InternalServerError);
        }


        var mail = new PlainTextMail(from, result.Email).WithSubject(@"Welcome to Career Compass")
            .WithBody($@"
            Welcome to Career Compass! Please click the link below to verify your email address. 
            {confirmUrl}
            ");

        await emailSender.Send(mail, cancellationToken);
        return Ok(new RegisterResponse
        {
            Message = "User registered successfully. Please check your email for verification."
        });
    }

    private string GenerateConfirmationUrl(RegisterCommandResult result, string returnUrl)
    {
        var request = HttpContext.Request;
        var scheme = request.Scheme;
        var host = request.Host.ToUriComponent();
        var uriBuilder = new UriBuilder
        {
            Scheme = scheme,
            Host = host,
            Path = $"users/confirm-email/{result.UserId}/{result.ConfirmationCode}",
            Query = "returnUrl=" + returnUrl
        };


        return uriBuilder.Uri.ToString();
    }

    [AllowAnonymous]
    [HttpGet("confirm-email/{userId:guid:required}/{code:required}")]
    public async Task<IActionResult> ConfirmEmail(Guid userId, string code, string returnUrl)
    {
        var input = new ConfirmEmailCommand(UserId.Create(userId), code);
        var result = await Context.Sender.Send(input);

        if (result.IsError)
        {
            return result.Errors.ToProblemDetails().ToActionResult();
        }

        return Redirect(returnUrl);
    }
}