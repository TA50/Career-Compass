using MediatR;
using ErrorOr;

namespace CareerCompass.Application.Scenarios.Queries.GetScenariosQuery;

public record GetScenariosQuery(string UserId, IList<string>? TagIds = null) : IRequest<ErrorOr<IList<Scenario>>>;