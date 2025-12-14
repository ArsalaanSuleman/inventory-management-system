namespace Inventory.Domain.Products;

// Represents a Stock Keeping Unit (SKU) identifier for products.
// SKUs are alphanumeric strings used to uniquely identify products in inventory.
// This record struct ensures that SKUs are at least 3 characters long and are stored in uppercase.

// Key behaviors:
//new Sku("abc-123") becomes ABC-123
//SKUs compare by value, not by reference

public readonly record struct Sku
{
    public string Value { get; }

    public Sku(string value)
    {
        value = (value ?? string.Empty).Trim();

        if (value.Length < 3)
            throw new ArgumentException("SKU must be at least 3 characters long.", nameof(value));
        
        Value = value.ToUpperInvariant();
    }

    public override string ToString() => Value;
}