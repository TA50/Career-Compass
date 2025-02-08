using CareerCompass.Core.Users;
using MediatR;
using ErrorOr;

namespace CareerCompass.Core.Scenarios.Queries.GetScenariosQuery;

public record GetScenariosQuery(UserId UserId, IList<string>? TagIds = null) : IRequest<ErrorOr<IList<Scenario>>>;