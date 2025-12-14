namespace Inventory.Domain.Common;

// Base class for all domain events in the system.
// Key properties:
// - EventId: Unique identifier for the event.
// - OccurredAt: Timestamp when the event occurred.
// - EventType: Type of the event, to be implemented by derived classes.

public abstract record DomainEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTimeOffset OccurredAt { get; init; } = DateTimeOffset.UtcNow;
    public abstract string EventType { get; }
}
