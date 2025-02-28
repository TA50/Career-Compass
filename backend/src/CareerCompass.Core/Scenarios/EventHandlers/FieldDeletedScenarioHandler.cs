using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Common.Abstractions.Repositories;
using CareerCompass.Core.Common.Specifications.Scenarios;
using CareerCompass.Core.Events;
using MediatR;

namespace CareerCompass.Core.Scenarios.EventHandlers;

public class FieldDeletedScenarioHandler(
    IScenarioRepository scenarioRepository,
    ILoggerAdapter<FieldDeletedScenarioHandler> logger) : INotificationHandler<FieldDeletedEvent>
{
    public async Task Handle(FieldDeletedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling {EventName} for field with id: {FieldId}", notification.GetType().Name,
            notification.FieldId);

        var spec = new GetScenariosSpecification();
        spec.WithFields([notification.FieldId]);
        var scenarios = await scenarioRepository.Get(spec, true, cancellationToken);

        foreach (var scenario in scenarios)
        {
            scenario.RemoveScenarioField(notification.FieldId);
        }

        await scenarioRepository.Save(cancellationToken);

        logger.LogInformation("Successfully handled {EventName} for field with id: {FieldId}",
            notification.GetType().Name,
            notification.FieldId);
    }
}