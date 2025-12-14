namespace Inventory.Domain.Orders;

// Represents the various statuses an order can have in the system.
// Key statuses:
// Draft: Initial state when an order is being created.
// Placed: Order has been finalized and placed.
// Reserved: Stock has been reserved for the order.
// Shipped: Order has been shipped to the customer.
// Cancelled: Order has been cancelled.

public enum OrderStatus
{
    Draft = 0,
    Placed = 1,
    Reserved = 2,
    Shipped = 3,
    Cancelled = 4
}