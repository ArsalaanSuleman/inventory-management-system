using Inventory.Domain.Products;
using Inventory.Domain.Warehouses;
using Inventory.Domain.Common;
using Inventory.Domain.Events;

namespace Inventory.Domain.Orders;

// Represents a customer order in the inventory system.
// Key behaviors:
// Create a new order for a customer.
// Add, remove, and change quantities of order lines while in Draft status.
// Place the order, changing its status to Placed and emitting an OrderPlaced event.
// Mark the order as Reserved or Shipped, updating its status accordingly.
// Cancel the order unless it has already been shipped.
// Emits domain events to signal important state changes.
// Enforces business rules around order status transitions and line item management.
// Example usage:
// var order = Order.Create(customerId: 1);
// order.AddLine(new Sku("ABC-123"), new Quantity(5));
// order.Place();
// order.MarkReserved();

public sealed class Order : AggregateRoot
{
    public int Id { get; private set; } 
    public int CustomerId { get; }
    public OrderStatus Status { get; private set; }

    private readonly List<OrderLine> _lines = new();
    public IReadOnlyList<OrderLine> Lines => _lines;

    private int _nextLineId = 1;

    private Order(int customerId)
    {
        if (customerId <= 0) throw new ArgumentOutOfRangeException(nameof(customerId));
        CustomerId = customerId;
        Status = OrderStatus.Draft;
    }

    public static Order Create(int customerId) => new(customerId);

    public void AddLine(Sku sku, Quantity qty)
    {
        EnsureEditable();

        if (qty.Value <= 0)
            throw new ArgumentOutOfRangeException(nameof(qty), "Quantity must be > 0.");

        var existing = _lines.FirstOrDefault(l => l.Sku.Equals(sku));
        if (existing is not null)
        {
            existing.Increase(qty);
            return;
        }

        _lines.Add(new OrderLine(_nextLineId++, sku, qty));
    }

    public void RemoveLine(Sku sku)
    {
        EnsureEditable();

        var line = _lines.FirstOrDefault(l => l.Sku.Equals(sku));
        if (line is null) return;

        _lines.Remove(line);
    }

    public void ChangeQuantity(Sku sku, Quantity newQty)
    {
        EnsureEditable();

        var line = _lines.FirstOrDefault(l => l.Sku.Equals(sku))
            ?? throw new InvalidOperationException($"SKU {sku} not found in order.");

        if (newQty.Value <= 0)
        {
            _lines.Remove(line); 
            return;
        }

        if (newQty.Value > line.Quantity.Value)
            line.Increase(new Quantity(newQty.Value - line.Quantity.Value));
        else if (newQty.Value < line.Quantity.Value)
            line.Decrease(new Quantity(line.Quantity.Value - newQty.Value));
    }

    public void Place()
    {
        if (Status != OrderStatus.Draft)
            throw new InvalidOperationException($"Cannot place order in status {Status}.");

        if (_lines.Count == 0)
            throw new InvalidOperationException("Cannot place an order with zero lines.");

        Status = OrderStatus.Placed;

        Raise(new OrderPlaced(OrderId: Id, CustomerId: CustomerId));
    }

    public void MarkReserved()
    {
        if (Status != OrderStatus.Placed)
            throw new InvalidOperationException($"Cannot mark reserved in status {Status}.");

        Status = OrderStatus.Reserved;
    }

    public void MarkShipped()
    {
        if (Status != OrderStatus.Reserved)
            throw new InvalidOperationException($"Cannot mark shipped in status {Status}.");

        Status = OrderStatus.Shipped;
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Shipped)
            throw new InvalidOperationException("Cannot cancel a shipped order.");

        Status = OrderStatus.Cancelled;
    }

    private void EnsureEditable()
    {
        if (Status != OrderStatus.Draft)
            throw new InvalidOperationException("Order can only be edited while in Draft status.");
    }
}
