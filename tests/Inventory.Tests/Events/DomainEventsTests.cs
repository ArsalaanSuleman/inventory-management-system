using Inventory.Domain.Events;
using Inventory.Domain.Orders;
using Inventory.Domain.Products;
using Inventory.Domain.Warehouses;
using Xunit;

namespace Inventory.Tests.Events;

public class DomainEventsTests
{
    [Fact]
    public void Order_Place_Emits_OrderPlaced()
    {
        var order = Order.Create(customerId: 1);
        order.AddLine(new Sku("ABC-123"), new Quantity(1));

        order.Place();

        Assert.Contains(order.DomainEvents, e => e is OrderPlaced);
    }

    [Fact]
    public void Warehouse_ReserveStock_Emits_StockReserved()
    {
        var wh = new Warehouse(1, "Main");
        var sku = new Sku("ABC-123");
        wh.AddStock(sku, new Quantity(10));

        wh.ReserveStock(orderId: 42, sku: sku, qty: new Quantity(3));

        Assert.Contains(wh.DomainEvents, e => e is StockReserved);
    }
}
