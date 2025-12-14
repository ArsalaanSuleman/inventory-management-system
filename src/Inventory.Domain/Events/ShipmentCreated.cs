using Inventory.Domain.Common;

namespace Inventory.Domain.Events;

// Event representing that a shipment has been created.
// Key properties:
// - ShipmentId: Identifier of the created shipment.
// - OrderId: Identifier of the order associated with the shipment.
// - WarehouseId: Identifier of the warehouse from which the shipment is made.
// This event is typically used to notify other parts of the system about the creation of a shipment.

public sealed record ShipmentCreated(int ShipmentId, int OrderId, int WarehouseId) : DomainEvent
{
    public override string EventType => nameof(ShipmentCreated);
}
