using FluentValidation;

namespace CareerCompass.Core.Scenarios.Queries.GetScenarioByIdQuery;

public class GetScenarioByIdQueryValidator : AbstractValidator<GetScenarioByIdQuery>
{
    public GetScenarioByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
    }
}