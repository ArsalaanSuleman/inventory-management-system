using Inventory.Domain.Orders;
using Inventory.Domain.Products;
using Inventory.Domain.Warehouses;
using Inventory.Domain.Common;
using Inventory.Domain.Events;


namespace Inventory.Domain.Shipping;

// Represents a shipment created for an order from a specific warehouse.
// Key properties:
// - Id: Unique identifier of the shipment.
// - OrderId: Identifier of the order associated with the shipment.
// - WarehouseId: Identifier of the warehouse from which the shipment is made.
// - CreatedAt: Timestamp when the shipment was created.
// - Lines: List of shipment lines detailing the SKUs and quantities being shipped.
// Key behaviors:
// - Create a shipment from an order and warehouse, adding shipment lines.
// - Emit a ShipmentCreated event upon creation.
// Example usage:
// var shipment = Shipment.FromOrder(orderId: 1, warehouseId: 2, lines: orderLines, createdAt: DateTimeOffset.UtcNow);
// shipment.AddLine(new Sku("ABC-123"), new Quantity(5));
// shipment.Raise(new ShipmentCreated(shipment.Id, orderId: 1, warehouseId: 2));
// Note: Shipment lines are aggregated within the shipment and cannot be modified directly after creation.
// Shipment lines are represented by the ShipmentLine class.
// Shipment lines contain:
// - Id: Unique identifier of the shipment line.
// - Sku: The SKU of the product being shipped.
// - Quantity: The quantity of the SKU being shipped.

public sealed class Shipment : AggregateRoot
{
    public int Id { get; internal set; }
    public int OrderId { get; }
    public int WarehouseId { get; }
    public DateTimeOffset CreatedAt { get; }

    private readonly List<ShipmentLine> _lines = new();
    public IReadOnlyList<ShipmentLine> Lines => _lines;

    private int _nextLineId = 1;

    private Shipment(int orderId, int warehouseId, DateTimeOffset createdAt)
    {
        if (orderId <= 0) throw new ArgumentOutOfRangeException(nameof(orderId));
        if (warehouseId <= 0) throw new ArgumentOutOfRangeException(nameof(warehouseId));

        OrderId = orderId;
        WarehouseId = warehouseId;
        CreatedAt = createdAt;
    }

    public static Shipment FromOrder(int orderId, int warehouseId, IEnumerable<(Sku sku, Quantity qty)> lines, DateTimeOffset createdAt)
    {
        var shipment = new Shipment(orderId, warehouseId, createdAt);

        foreach (var (sku, qty) in lines)
            shipment.AddLine(sku, qty);

        if (shipment._lines.Count == 0)
            throw new InvalidOperationException("Cannot create shipment with zero lines.");

        shipment.Raise(new ShipmentCreated(ShipmentId: shipment.Id, OrderId: orderId, WarehouseId: warehouseId));

        return shipment;
    }

    private void AddLine(Sku sku, Quantity qty)
    {
        var existing = _lines.FirstOrDefault(l => l.Sku.Equals(sku));
        if (existing is not null)
        {
            _lines.Remove(existing);
            _lines.Add(new ShipmentLine(existing.Id, sku, new Quantity(existing.Quantity.Value + qty.Value)));
            return;
        }

        _lines.Add(new ShipmentLine(_nextLineId++, sku, qty));
    }
}
