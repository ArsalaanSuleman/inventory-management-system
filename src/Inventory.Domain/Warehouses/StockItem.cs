using Inventory.Domain.Products;

namespace Inventory.Domain.Warehouses;

// Represents a stock item in the warehouse, tracking on-hand and reserved quantities.
// Key properties:
// - Sku: The SKU of the product.
// - OnHand: The total quantity of the product physically present in the warehouse.
// - Reserved: The quantity of the product that has been reserved for orders.
// - Available: The quantity of the product that is available for reservation (OnHand - Reserved).
// Key behaviors:
// - AddStock: Increases the OnHand quantity.
// - Reserve: Increases the Reserved quantity, ensuring it does not exceed Available.
// - Unreserve: Decreases the Reserved quantity.
// - ShipReserved: Decreases both OnHand and Reserved quantities when stock is shipped.


public sealed class StockItem
{
    public Sku Sku { get; }
    public Quantity OnHand { get; private set; }
    public Quantity Reserved { get; private set; }

    public Quantity Available => new(OnHand.Value - Reserved.Value);

    public StockItem(Sku sku, Quantity onHand)
    {
        Sku = sku;
        OnHand = onHand;
        Reserved = new Quantity(0);
    }

    public void AddStock(Quantity qty)
    {
        if (qty.Value <= 0) throw new ArgumentOutOfRangeException(nameof(qty), "Quantity must be > 0.");
        OnHand += qty;
    }

    public void Reserve(Quantity qty)
    {
        if (qty.Value <= 0) throw new ArgumentOutOfRangeException(nameof(qty), "Quantity must be > 0.");
        if (qty.Value > Available.Value) throw new InvalidOperationException("Not enough available stock to reserve.");
        Reserved += qty;
    }

    public void Unreserve(Quantity qty)
    {
        if (qty.Value <= 0) throw new ArgumentOutOfRangeException(nameof(qty), "Quantity must be > 0.");
        if (qty.Value > Reserved.Value) throw new InvalidOperationException("Cannot unreserve more than reserved.");
        Reserved -= qty;
    }

    public void ShipReserved(Quantity qty)
    {
        if (qty.Value <= 0) throw new ArgumentOutOfRangeException(nameof(qty), "Quantity must be > 0.");
        if (qty.Value > Reserved.Value) throw new InvalidOperationException("Cannot ship more than reserved.");

        Reserved -= qty;
        OnHand -= qty;
    }
}
