using CareerCompass.Core.Common;
using CareerCompass.Core.Common.Extensions;
using FluentValidation;

namespace CareerCompass.Core.Scenarios.Commands.UpdateScenario;

public class UpdateScenarioFieldCommandValidator : AbstractValidator<UpdateScenarioFieldCommand>
{
    public UpdateScenarioFieldCommandValidator()
    {
        RuleFor(x => x.Value)
            .NotEmpty();

        RuleFor(x => x.FieldId)
            .NotEmpty();
    }
}

public class UpdateScenarioCommandValidator : AbstractValidator<UpdateScenarioCommand>
{
    public UpdateScenarioCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(100);
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleForEach(x => x.TagIds)
            .NotEmpty();

        RuleForEach(x => x.ScenarioFields)
            .SetValidator(new UpdateScenarioFieldCommandValidator());

        // Field Ids are distinct: 
        RuleFor(x => x.ScenarioFields)
            .Must(x => x.IsDistinct(t => t.FieldId))
            .WithMessage("Scenario Field Ids must be distinct");
    }
}