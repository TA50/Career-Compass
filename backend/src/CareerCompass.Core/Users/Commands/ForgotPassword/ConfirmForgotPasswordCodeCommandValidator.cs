using FluentValidation;

namespace CareerCompass.Core.Users.Commands.ForgotPassword;

public class ConfirmForgotPasswordCodeCommandValidator : AbstractValidator<ConfirmForgotPasswordCodeCommand>
{
    public ConfirmForgotPasswordCodeCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Code)
            .NotEmpty();
    }
}