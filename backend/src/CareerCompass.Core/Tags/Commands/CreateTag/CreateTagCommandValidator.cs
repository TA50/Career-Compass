using CareerCompass.Core.Common;
using FluentValidation;

namespace CareerCompass.Core.Tags.Commands.CreateTag;

public class CreateTagCommandValidator : AbstractValidator<CreateTagCommand>
{
    public CreateTagCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty()
            .MaximumLength(Limits.MaxScenarioTitleLength);
    }
}