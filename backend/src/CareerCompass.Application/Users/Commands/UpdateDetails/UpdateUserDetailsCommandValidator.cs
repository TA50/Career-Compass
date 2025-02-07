using CareerCompass.Application.Common;
using FluentValidation;

namespace CareerCompass.Application.Users.Commands.UpdateDetails;

public class UpdateUserDetailsCommandValidator : AbstractValidator<UpdateUserDetailsCommand>
{
    public UpdateUserDetailsCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .IsGuid();
    }
}