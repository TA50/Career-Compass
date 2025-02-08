using CareerCompass.Core.Common;
using CareerCompass.Core.Common.Extensions;
using FluentValidation;

namespace CareerCompass.Core.Scenarios.Commands.CreateScenario;

public class CreateScenarioFieldCommandValidator : AbstractValidator<CreateScenarioFieldCommand>
{
    public CreateScenarioFieldCommandValidator()
    {
        RuleFor(x => x.Value)
            .NotEmpty();

        RuleFor(x => x.FieldId)
            .NotEmpty();
    }
}

public class CreateScenarioCommandValidator : AbstractValidator<CreateScenarioCommand>
{
    public CreateScenarioCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleForEach(x => x.TagIds)
            .NotEmpty();

        RuleForEach(x => x.ScenarioFields)
            .SetValidator(new CreateScenarioFieldCommandValidator());

        // Field Ids are distinct: 
        RuleFor(x => x.ScenarioFields)
            .IsDistinct(t => t.FieldId)
            .WithMessage("Scenario Field Ids must be distinct");
    }
}