using Inventory.Domain.Common;

namespace Inventory.Domain.Events;

// Event representing that an order has been placed.
// Key properties:
// - OrderId: Identifier of the placed order.
// - CustomerId: Identifier of the customer who placed the order.
// This event is typically used to trigger subsequent processes such as stock reservation and shipment creation.

public sealed record OrderPlaced(int OrderId, int CustomerId) : DomainEvent
{
    public override string EventType => nameof(OrderPlaced);
}
