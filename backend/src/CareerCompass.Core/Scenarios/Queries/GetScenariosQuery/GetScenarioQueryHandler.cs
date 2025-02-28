using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Common.Abstractions.Repositories;
using CareerCompass.Core.Common.Models;
using CareerCompass.Core.Common.Specifications.Scenarios;
using MediatR;
using ErrorOr;

namespace CareerCompass.Core.Scenarios.Queries.GetScenariosQuery;

public class GetScenarioQueryHandler(
    IScenarioRepository scenarioRepository,
    ILoggerAdapter<GetScenarioQueryHandler> logger)
    : IRequestHandler<GetScenariosQuery, ErrorOr<PaginationResult<Scenario>>>
{
    public async Task<ErrorOr<PaginationResult<Scenario>>> Handle(GetScenariosQuery request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting scenarios for user {@UserId}", request.UserId);
        var totalItems =
            await scenarioRepository.Count(new GetScenariosSpecification(request.UserId), cancellationToken);
        if (totalItems == 0)
        {
            logger.LogInformation("No scenarios found for user {@UserId}", request.UserId);
            return ErrorOrFactory.From(PaginationResult<Scenario>.Empty);
        }


        var spec = new GetScenariosSpecification();
        spec.BelongsTo(request.UserId);
        if (request.TagIds is not null && request.TagIds.Any())
        {
            spec.WithTags(request.TagIds);
        }

        var page = 1;
        var pageSize = totalItems;

        if (request is { Page: not null, PageSize: not null })
        {
            page = request.Page.Value;
            pageSize = request.PageSize.Value;
        }

        spec.WithPagination(page, pageSize);

        var items = await scenarioRepository.Get(spec, cancellationToken);
        var result = new PaginationResult<Scenario>(items, totalItems, pageSize: pageSize, page: page);

        logger.LogInformation("Found {TotalItems} scenarios for user {UserId} on page {Page} with page size {PageSize}",
            totalItems, request.UserId, page, pageSize);

        return ErrorOrFactory.From(result);
    }
}