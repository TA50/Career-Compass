using FluentValidation;

namespace CareerCompass.Core.Scenarios.Queries.GetScenariosQuery;

public class GetScenariosQueryValidator : AbstractValidator<GetScenariosQuery>
{
    public GetScenariosQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}