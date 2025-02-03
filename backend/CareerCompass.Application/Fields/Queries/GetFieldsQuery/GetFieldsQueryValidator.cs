using CareerCompass.Application.Common;
using FluentValidation;

namespace CareerCompass.Application.Fields.Queries.GetFieldsQuery;

public class GetFieldsQueryValidator : AbstractValidator<GetFieldsQuery>
{
    public GetFieldsQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .IsGuid();
    }
}