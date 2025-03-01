using CareerCompass.Core.Common;
using FluentValidation;

namespace CareerCompass.Core.Fields.Commands.CreateField;

public class CreateFieldCommandValidator : AbstractValidator<CreateFieldCommand>
{
    public CreateFieldCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(Limits.MaxNameLength);
        RuleFor(x => x.Group)
            .NotEmpty()
            .MaximumLength(Limits.MaxNameLength);
    }
}