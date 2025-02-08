using CareerCompass.Core.Common;
using CareerCompass.Core.Common.Extensions;
using FluentValidation;

namespace CareerCompass.Core.Tags.Queries.GetTagsQuery;

public class GetTagsQueryValidator : AbstractValidator<GetTagsQuery>
{
    public GetTagsQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        
    }
}