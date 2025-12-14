using Inventory.Domain.Products;
using Inventory.Domain.Common;
using Inventory.Domain.Events;

namespace Inventory.Domain.Warehouses;

// Represents a warehouse that holds stock of products.
// Key properties:
// - Id: Unique identifier of the warehouse.
// - Name: Name of the warehouse.
// - Stock: Collection of stock items available in the warehouse.   
// Key behaviors:
// - AddStock: Adds stock for a specific SKU.
// - ReserveStock: Reserves stock for an order, emitting a StockReserved event.
// - UnreserveStock: Unreserves previously reserved stock.
// - ShipReservedStock: Ships reserved stock, reducing on-hand and reserved quantities.
// Example usage:
// var warehouse = new Warehouse(id: 1, name: "Main Warehouse");
// warehouse.AddStock(new Sku("ABC-123"), new Quantity(10));
// warehouse.ReserveStock(orderId: 42, new Sku("ABC-123"), new Quantity(3));
// warehouse.UnreserveStock(new Sku("ABC-123"), new Quantity(2));
// warehouse.ShipReservedStock(new Sku("ABC-123"), new Quantity(1));

public sealed class Warehouse : AggregateRoot
{
    public int Id { get; }
    public string Name { get; }

    private readonly Dictionary<Sku, StockItem> _stock = new();
    public IReadOnlyCollection<StockItem> Stock => _stock.Values;

    public Warehouse(int id, string name)
    {
        if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
        name = (name ?? string.Empty).Trim();
        if (name.Length == 0) throw new ArgumentException("Warehouse name required.", nameof(name));

        Id = id;
        Name = name;
    }

    public StockItem GetOrCreateStockItem(Sku sku)
    {
        if (_stock.TryGetValue(sku, out var item)) return item;

        item = new StockItem(sku, new Quantity(0));
        _stock[sku] = item;
        return item;
    }

    public void AddStock(Sku sku, Quantity qty)
    {
        var item = GetOrCreateStockItem(sku);
        item.AddStock(qty);
    }

    public void ReserveStock(int orderId,Sku sku, Quantity qty)
    {

        if (orderId <= 0) throw new ArgumentOutOfRangeException(nameof(orderId));

        if (!_stock.TryGetValue(sku, out var item))
            throw new InvalidOperationException($"No stock for SKU {sku}.");

        item.Reserve(qty);

        Raise(new StockReserved(
        OrderId: orderId,
        WarehouseId: Id,
        Sku: sku,
        Quantity: qty
    ));
    }

    public void UnreserveStock(Sku sku, Quantity qty)
    {
        if (!_stock.TryGetValue(sku, out var item))
            throw new InvalidOperationException($"No stock for SKU {sku}.");

        item.Unreserve(qty);
    }

    public void ShipReservedStock(Sku sku, Quantity qty)
    {
        if (!_stock.TryGetValue(sku, out var item))
            throw new InvalidOperationException($"No stock for SKU {sku}.");

        item.ShipReserved(qty);
    }
}
