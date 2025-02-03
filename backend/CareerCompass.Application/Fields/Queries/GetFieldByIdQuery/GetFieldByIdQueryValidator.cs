using CareerCompass.Application.Common;
using CareerCompass.Application.Tags.Queries.GetTagByIdQuery;
using FluentValidation;

namespace CareerCompass.Application.Fields.Queries.GetFieldByIdQuery;

public class GetFieldByIdQueryValidator : AbstractValidator<GetFieldByIdQuery>
{
    public GetFieldByIdQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .IsGuid();

        RuleFor(x => x.FieldId)
            .NotEmpty();

        RuleFor(x => x.FieldId)
            .IsGuid();
    }
}