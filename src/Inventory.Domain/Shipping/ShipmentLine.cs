using Inventory.Domain.Products;
using Inventory.Domain.Warehouses;

namespace Inventory.Domain.Shipping;

// Represents a line item within a shipment.
// Each shipment line contains details about the SKU being shipped and the quantity.
// Key behaviors:
// - Creation with a specific SKU and quantity.
// - Quantity must always be positive when created.
// Example usage:
// var line = new ShipmentLine(1, new Sku("ABC-123"), new Quantity(5));
// Note: Shipment lines are immutable once created.
// Shipment lines are created and managed by the Shipment aggregate.
// Shipment lines cannot be modified directly after creation.
// Shipment lines are represented by the ShipmentLine class.
// Shipment lines contain:
// - Id: Unique identifier of the shipment line.
// - Sku: The SKU of the product being shipped.
// - Quantity: The quantity of the SKU being shipped.
// Note: Shipment lines are created and managed by the Shipment aggregate.
// They cannot be modified directly after creation.
// Example usage:
// var shipment = Shipment.FromOrder(orderId: 1, warehouseId: 2, lines: orderLines, createdAt: DateTimeOffset.UtcNow);
// shipment.AddLine(new Sku("ABC-123"), new Quantity(5));
// shipment.Raise(new ShipmentCreated(shipment.Id, orderId: 1, warehouseId: 2));

public sealed class ShipmentLine
{
    public int Id { get; }
    public Sku Sku { get; }
    public Quantity Quantity { get; }

    internal ShipmentLine(int id, Sku sku, Quantity qty)
    {
        if (qty.Value <= 0) throw new ArgumentOutOfRangeException(nameof(qty), "Quantity must be > 0.");

        Id = id;
        Sku = sku;
        Quantity = qty;
    }
}
