using CareerCompass.Core.Common;
using CareerCompass.Core.Common.Extensions;
using FluentValidation;

namespace CareerCompass.Core.Tags.Queries.GetTagByIdQuery;

public class GetTagByIdQueryValidator : AbstractValidator<GetTagByIdQuery>
{
    public GetTagByIdQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();


        RuleFor(x => x.TagId)
            .NotEmpty();
    }
}