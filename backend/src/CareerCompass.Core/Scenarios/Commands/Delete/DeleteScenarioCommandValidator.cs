using FluentValidation;

namespace CareerCompass.Core.Scenarios.Commands.Delete;

public class DeleteScenarioCommandValidator : AbstractValidator<DeleteScenarioCommand>
{
    public DeleteScenarioCommandValidator()
    {
        RuleFor(x => x.ScenarioId)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}