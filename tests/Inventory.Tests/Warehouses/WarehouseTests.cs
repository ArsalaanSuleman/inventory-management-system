using Inventory.Domain.Products;
using Inventory.Domain.Warehouses;
using Xunit;

namespace Inventory.Tests.Warehouses;

public class WarehouseTests
{
    [Fact]
    public void ReserveStock_WithinAvailable_Succeeds()
    {
        var wh = new Warehouse(1, "Main");
        var sku = new Sku("ABC-123");

        wh.AddStock(sku, new Quantity(10));
        wh.ReserveStock(orderId: 42, sku: sku, qty: new Quantity(3));

        var item = wh.GetOrCreateStockItem(sku);

        Assert.Equal(10, item.OnHand.Value);
        Assert.Equal(3, item.Reserved.Value);
        Assert.Equal(7, item.Available.Value);
    }

    [Fact]
    public void ReserveStock_MoreThanAvailable_Throws()
    {
        var wh = new Warehouse(1, "Main");
        var sku = new Sku("ABC-123");

        wh.AddStock(sku, new Quantity(5));

        Assert.Throws<InvalidOperationException>(() =>
            wh.ReserveStock(orderId: 42, sku: sku, qty: new Quantity(6)));

    }

    [Fact]
    public void ShipReservedStock_ReducesOnHandAndReserved()
    {
        var wh = new Warehouse(1, "Main");
        var sku = new Sku("ABC-123");

        wh.AddStock(sku, new Quantity(10));
        wh.ReserveStock(orderId: 42, sku: sku, qty: new Quantity(4));
        wh.ShipReservedStock(sku, new Quantity(4));

        var item = wh.GetOrCreateStockItem(sku);

        Assert.Equal(6, item.OnHand.Value);
        Assert.Equal(0, item.Reserved.Value);
        Assert.Equal(6, item.Available.Value);
    }
}
