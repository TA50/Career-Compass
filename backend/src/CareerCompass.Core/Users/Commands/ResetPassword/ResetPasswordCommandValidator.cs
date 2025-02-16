using CareerCompass.Core.Common;
using FluentValidation;

namespace CareerCompass.Core.Users.Commands.ResetPassword;

public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.OldPassword)
            .NotEmpty();

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .Matches(RegexPatterns.Password);

        RuleFor(x => x.ConfirmNewPassword)
            .Equal(x => x.NewPassword);
    }
}