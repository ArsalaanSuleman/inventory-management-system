namespace Inventory.Domain.Common;

// Base class for aggregate roots in the domain-driven design context.
// An aggregate root is an entity that serves as the entry point for a cluster of related objects (the aggregate).
// It is responsible for maintaining the integrity of the aggregate and ensuring that all business rules are enforced.
// This class provides functionality to manage domain events that occur within the aggregate.
/// Key behaviors:
// - Raise domain events to signal important changes within the aggregate.
// - Clear domain events after they have been processed.
/// Example usage:
// public class Order : AggregateRoot
// {
//     public void PlaceOrder()
//     {
//         // Business logic for placing an order
//         Raise(new OrderPlacedEvent(this.Id));
//     }
// }    

public abstract class AggregateRoot
{
    private readonly List<DomainEvent> _domainEvents = new();

    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void Raise(DomainEvent ev) => _domainEvents.Add(ev);

    public void ClearDomainEvents() => _domainEvents.Clear();
}
