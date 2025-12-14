using Inventory.Domain.Common;
using Inventory.Domain.Products;
using Inventory.Domain.Warehouses;

namespace Inventory.Domain.Events;

// Event representing that stock has been reserved for a specific order.
// Contains details about the order, warehouse, SKU, and quantity reserved.
// Key properties:
// - OrderId: Identifier of the order for which stock is reserved.
// - WarehouseId: Identifier of the warehouse where stock is reserved.
// - Sku: The SKU of the product for which stock is reserved.
// - Quantity: The quantity of stock that has been reserved.

public sealed record StockReserved(
    int OrderId,
    int WarehouseId,
    Sku Sku,
    Quantity Quantity
) : DomainEvent
{
    public override string EventType => nameof(StockReserved);
}
