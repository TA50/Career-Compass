using FluentValidation;

namespace CareerCompass.Core.Users.Commands.ForgotPassword;

public class GenerateForgotPasswordCodeCommandValidator : AbstractValidator<GenerateForgotPasswordCodeCommand>
{
    public GenerateForgotPasswordCodeCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
    }
}