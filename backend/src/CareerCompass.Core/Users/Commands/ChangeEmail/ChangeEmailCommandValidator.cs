using CareerCompass.Core.Common;
using FluentValidation;

namespace CareerCompass.Core.Users.Commands.ChangeEmail;

public class ChangeEmailCommandValidator : AbstractValidator<ChangeEmailCommand>
{
    public ChangeEmailCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.OldPassword)
            .NotEmpty();

        RuleFor(x => x.NewEmail)
            .NotEmpty()
            .EmailAddress();
    }
}