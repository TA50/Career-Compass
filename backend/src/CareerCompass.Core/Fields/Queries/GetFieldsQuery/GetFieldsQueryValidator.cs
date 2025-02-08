using CareerCompass.Core.Common.Extensions;
using FluentValidation;

namespace CareerCompass.Core.Fields.Queries.GetFieldsQuery;

public class GetFieldsQueryValidator : AbstractValidator<GetFieldsQuery>
{
    public GetFieldsQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}