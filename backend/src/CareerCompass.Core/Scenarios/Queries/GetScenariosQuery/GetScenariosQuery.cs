using CareerCompass.Core.Common.Models;
using CareerCompass.Core.Tags;
using CareerCompass.Core.Users;
using MediatR;
using ErrorOr;

namespace CareerCompass.Core.Scenarios.Queries.GetScenariosQuery;

public record GetScenariosQuery(
    UserId UserId,
    IList<TagId>? TagIds = null,
    int? Page = null,
    int? PageSize = null
) : IRequest<ErrorOr<PaginationResult<Scenario>>>;