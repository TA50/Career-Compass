using CareerCompass.Application.Common;
using FluentValidation;

namespace CareerCompass.Application.Tags.Queries.GetTagsQuery;

public class GetTagsQueryValidator : AbstractValidator<GetTagsQuery>
{
    public GetTagsQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .IsGuid();
    }
}