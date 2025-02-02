using MediatR;
using ErrorOr;

namespace CareerCompass.Application.Scenarios.Queries.GetScenariosQuery;

public record GetScenariosQuery(string UserId) : IRequest<ErrorOr<IList<Scenario>>>;