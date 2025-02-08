using CareerCompass.Core.Common;
using CareerCompass.Core.Common.Extensions;
using CareerCompass.Core.Tags.Queries.GetTagByIdQuery;
using FluentValidation;

namespace CareerCompass.Core.Fields.Queries.GetFieldByIdQuery;

public class GetFieldByIdQueryValidator : AbstractValidator<GetFieldByIdQuery>
{
    public GetFieldByIdQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.FieldId)
            .NotEmpty();
    }
}