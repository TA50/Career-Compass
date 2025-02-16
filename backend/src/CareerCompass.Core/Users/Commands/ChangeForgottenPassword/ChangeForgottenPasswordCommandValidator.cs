using CareerCompass.Core.Common;
using FluentValidation;

namespace CareerCompass.Core.Users.Commands.ChangeForgottenPassword;

public class ChangeForgottenPasswordCommandValidator : AbstractValidator<ChangeForgottenPasswordCommand>
{
    public ChangeForgottenPasswordCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Code)
            .NotEmpty();

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .Matches(RegexPatterns.Password);

        RuleFor(x => x.ConfirmNewPassword)
            .Equal(x => x.NewPassword);
    }
}