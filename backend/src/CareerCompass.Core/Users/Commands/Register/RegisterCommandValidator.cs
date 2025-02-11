using CareerCompass.Core.Common;
using FluentValidation;

namespace CareerCompass.Core.Users.Commands.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(c => c.FirstName)
            .NotEmpty()
            .MaximumLength(Limits.MaxNameLength);

        RuleFor(c => c.LastName)
            .NotEmpty()
            .MaximumLength(Limits.MaxNameLength);

        RuleFor(c => c.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(Limits.MaxEmailLength);


        RuleFor(c => c.ConfirmPassword)
            .NotEmpty();

        RuleFor(c => c.Password)
            .Matches(RegexPatterns.Password)
            .NotEmpty()
            .Equal(c => c.ConfirmPassword)
            .MaximumLength(Limits.MaxPasswordLength);
    }
}