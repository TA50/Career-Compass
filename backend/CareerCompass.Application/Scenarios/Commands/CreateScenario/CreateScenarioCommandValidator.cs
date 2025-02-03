using CareerCompass.Application.Common;
using FluentValidation;

namespace CareerCompass.Application.Scenarios.Commands.CreateScenario;

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
            .NotEmpty();

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