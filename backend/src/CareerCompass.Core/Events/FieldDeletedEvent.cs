using CareerCompass.Core.Fields;
using MediatR;

namespace CareerCompass.Core.Events;

public record FieldDeletedEvent(FieldId FieldId) : INotification;