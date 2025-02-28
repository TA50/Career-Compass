using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Common.Abstractions.Repositories;
using CareerCompass.Core.Common.Specifications.Scenarios;
using CareerCompass.Core.Events;
using MediatR;

namespace CareerCompass.Core.Scenarios.EventHandlers;

public class TagDeletedScenarioHandler(
    IScenarioRepository scenarioRepository,
    ILoggerAdapter<TagDeletedScenarioHandler> logger) : INotificationHandler<TagDeletedEvent>
{
    public async Task Handle(TagDeletedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling {EventName} for field with id: {TagId}", notification.GetType().Name,
            notification.TagId);

        var spec = new GetScenariosSpecification();
        spec.WithTags([notification.TagId]);
        var scenarios = await scenarioRepository.Get(spec, true, cancellationToken);


        foreach (var scenario in scenarios)
        {
            scenario.RemoveTag(notification.TagId);
        }

        await scenarioRepository.Save(cancellationToken);

        logger.LogInformation("Successfully handled {EventName} for field with id: {TagId}",
            notification.GetType().Name,
            notification.TagId);
    }
}