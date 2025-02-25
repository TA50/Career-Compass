using CareerCompass.Core.Tags;
using MediatR;

namespace CareerCompass.Core.Events;

public record TagDeletedEvent(TagId TagId) : INotification;