using CareerCompass.Api.Contracts.Users;
using CareerCompass.Api.Extensions;
using CareerCompass.Api.Services;
using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Users;
using CareerCompass.Core.Users.Commands.ChangeEmail;
using CareerCompass.Core.Users.Commands.ChangeForgottenPassword;
using CareerCompass.Core.Users.Commands.ConfirmEmail;
using CareerCompass.Core.Users.Commands.GenerateForgotPasswordCode;
using CareerCompass.Core.Users.Commands.Login;
using CareerCompass.Core.Users.Commands.ResetPassword;
using CareerCompass.Core.Users.Queries.GetUserById;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace CareerCompass.Api.Controllers;

[ApiController]
[Route("users")]
public class UserController(
    ApiControllerContext context,
    AuthenticationEmailSender emailSender,
    ILoggerAdapter<UserController> logger)
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

    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> GetMe()
    {
        var userId = CurrentUserId;
        var input = new GetUserByIdQuery(userId);
        var result = await Context.Sender.Send(input);

        return result.Match(
            value => Ok(
                Context.Mapper.Map<UserDto>(value)
            ),
            error => error.ToProblemDetails()
                .ToActionResult<UserDto>());
    }

    #region Authentication

    [HttpGet("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> GenerateForgotPasswordCode(string email, string returnUrl,
        CancellationToken cancellationToken)
    {
        var input = new GenerateForgotPasswordCodeCommand(email);
        var result = await Context.Sender.Send(input);

        if (result.IsError)
        {
            return result.Errors.ToProblemDetails().ToActionResult();
        }

        var emailResult =
            await emailSender.SendForgotPasswordEmail(email, result.Value.Code, cancellationToken);

        return emailResult.Match(
            _ => Ok(),
            error => error.ToProblemDetails().ToActionResult()
        );
    }

    [HttpPost("change-forgotten-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ChangeForgottenPassword([FromBody] ChangeForgottenPasswordRequest dto,
        CancellationToken cancellationToken)
    {
        var input = new ChangeForgottenPasswordCommand(dto.Email, dto.Code, dto.NewPassword, dto.ConfirmNewPassword);
        var result = await Context.Sender.Send(input, cancellationToken);

        return result.Match(
            _ => Ok(),
            error => error.ToProblemDetails().ToActionResult()
        );
    }


    [HttpPost]
    [EndpointSummary("Register a new user")]
    [AllowAnonymous]
    public async Task<ActionResult<RegisterResponse>> Register([FromBody] RegisterRequest dto,
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
        var emailResult = await emailSender.SendConfirmationEmail(
            result.Value.Email, result.Value.ConfirmationCode,
            cancellationToken);


        return emailResult.Match(
            _ => Created(string.Empty, new RegisterResponse(
                "User has been registered successfully. Please confirm your email",
                result.Value.UserId.ToString())),
            error => error.ToProblemDetails()
                .ToActionResult<RegisterResponse>()
        );
    }


    [HttpPost("change-email")]
    public async Task<ActionResult<ChangeEmailResponse>> ChangeEmail([FromBody] ChangeEmailRequest dto,
        CancellationToken cancellationToken)
    {
        var input = new ChangeEmailCommand(CurrentUserId, dto.OldPassword, dto.Email);
        var result = await Context.Sender.Send(input, cancellationToken);

        if (result.IsError)
        {
            return result.Errors.ToProblemDetails()
                .ToActionResult<ChangeEmailResponse>();
        }

        // Send email
        var emailResult = await emailSender.SendConfirmationEmail(
            dto.Email, result.Value.EmailConfirmationCode, cancellationToken);

        if (emailResult.IsError)
        {
            return emailResult.Errors.ToProblemDetails()
                .ToActionResult<ChangeEmailResponse>();
        }

        await Logout();

        return Ok(new ChangeEmailResponse(
            "Email has been changed successfully. Please login with your new email after confirming it"));
    }

    [AllowAnonymous]
    [HttpGet("confirm-email/{userId:guid:required}/{code:required}", Name = nameof(ConfirmEmail))]
    public async Task<IActionResult> ConfirmEmail(Guid userId, string code)
    {
        var input = new ConfirmEmailCommand(UserId.Create(userId), code);
        var result = await Context.Sender.Send(input);

        return result.Match(
            _ => Ok(),
            error => error.ToProblemDetails().ToActionResult()
        );
    }


    [HttpPost("reset-password")]
    public async Task<ActionResult<ResetPasswordResponse>> ResetPassword([FromBody] ResetPasswordRequest dto,
        CancellationToken cancellationToken)
    {
        var input = new ResetPasswordCommand(CurrentUserId, dto.OldPassword, dto.NewPassword, dto.ConfirmNewPassword);
        var result = await Context.Sender.Send(input, cancellationToken);

        if (result.IsError)
        {
            return result.FirstError.ToProblemDetails()
                .ToActionResult<ResetPasswordResponse>();
        }

        await Logout();
        return Ok(
            new ResetPasswordResponse("Password has been reset successfully. Please login with your new password"));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest dto, CancellationToken cancellationToken)
    {
        if (IsAuthenticated)
        {
            return Ok();
        }

        var command = new LoginCommand(dto.Email, dto.Password);
        var result = await Context.Sender.Send(command, cancellationToken);

        if (result.IsError)
        {
            return result.Errors.ToProblemDetails().ToActionResult();
        }

        var principal = GenerateClaimsPrincipal(result.Value.UserId);
        await HttpContext.SignInAsync(principal);

        return Ok();
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok();
    }

    #endregion
}