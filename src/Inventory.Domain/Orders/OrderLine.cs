using Inventory.Domain.Products;
using Inventory.Domain.Warehouses;

namespace Inventory.Domain.Orders;

// Represents a line item in an order, specifying the product (SKU) and quantity ordered.
// Key behaviors:
// - Creation with a specific SKU and quantity.
// - Ability to increase or decrease the quantity, ensuring it does not go below zero.
// - Immutable SKU once created.
// - Quantity must always be positive when created or increased.
// - Decreasing quantity below zero throws an exception.
// Example usage:
// var line = new OrderLine(1, new Sku("ABC-123"), new Quantity(5));
// line.Increase(new Quantity(3)); // Quantity is now 8
// line.Decrease(new Quantity(2)); // Quantity is now 6

public sealed class OrderLine
{
    public int Id { get; }
    public Sku Sku { get; }
    public Quantity Quantity { get; private set; }

    internal OrderLine(int id, Sku sku, Quantity quantity)
    {
        Id = id;
        Sku = sku;
        Quantity = quantity;
    }

    internal void Increase(Quantity qty)
    {
        if(qty.Value <= 0)
            throw new ArgumentOutOfRangeException(nameof(qty), "Increase quantity must be positive.");
        Quantity += qty;

    }

    internal void Decrease(Quantity qty)
    {
        if(qty.Value <= 0)
            throw new ArgumentOutOfRangeException(nameof(qty), "Decrease quantity must be positive.");

        if (qty.Value > Quantity.Value)
            throw new InvalidOperationException("Cannot decrease below zero.");
    
        Quantity -= qty;
    }
}