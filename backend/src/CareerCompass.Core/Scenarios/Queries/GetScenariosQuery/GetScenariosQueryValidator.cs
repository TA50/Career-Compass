using FluentValidation;
using CareerCompass.Core.Common.Extensions;

namespace CareerCompass.Core.Scenarios.Queries.GetScenariosQuery;

public class GetScenariosQueryValidator : AbstractValidator<GetScenariosQuery>
{
    public GetScenariosQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.PageSize)
            .NotEmpty()
            .When(x => x.Page.HasValue);

        RuleFor(x => x.Page)
            .NotEmpty()
            .When(x => x.PageSize.HasValue);
    }
}