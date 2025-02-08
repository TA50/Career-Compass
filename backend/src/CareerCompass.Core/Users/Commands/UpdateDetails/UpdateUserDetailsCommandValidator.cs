using CareerCompass.Core.Common;
using CareerCompass.Core.Common.Extensions;
using FluentValidation;

namespace CareerCompass.Core.Users.Commands.UpdateDetails;

public class UpdateUserDetailsCommandValidator : AbstractValidator<UpdateUserDetailsCommand>
{
    public UpdateUserDetailsCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        
    }
}