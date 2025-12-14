namespace Inventory.Domain.Warehouses;

// Represents a quantity of items in inventory.
// Quantities cannot be negative.
// Key behaviors:
// Quantity q = new Quantity(5); // creates a quantity of 5
// q + new Quantity(3) results in Quantity of 8
// q - new Quantity(2) results in Quantity of 3
// Subtracting more than available throws InvalidOperationException

public readonly record struct Quantity
{
    public int Value {get;}

    public Quantity (int value)
    {
        if (value < 0) throw new ArgumentOutOfRangeException(nameof(value), "Quantity cannot be negative.");
        Value = value;
    }

    public static Quantity operator +(Quantity a, Quantity b) => new (a.Value + b.Value);
    public static Quantity operator -(Quantity a, Quantity b)
    {
        if (b.Value > a.Value) throw new InvalidOperationException("Cannot subtract more then available.");
        return new (a.Value - b.Value);
    }

    public override string ToString() => Value.ToString();

}