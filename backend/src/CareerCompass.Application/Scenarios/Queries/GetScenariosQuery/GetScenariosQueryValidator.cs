using FluentValidation;

namespace CareerCompass.Application.Scenarios.Queries.GetScenariosQuery;

public class GetScenariosQueryValidator : AbstractValidator<GetScenariosQuery>
{
    public GetScenariosQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}