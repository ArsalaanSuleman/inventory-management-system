using Inventory.Application.UseCases;
using Inventory.Domain.Orders;
using Inventory.Domain.Products;
using Inventory.Domain.Warehouses;
using Inventory.Tests.TestDoubles;
using Xunit;

namespace Inventory.Tests.UseCases;

public class CreateShipmentForOrderTests
{
    [Fact]
    public async Task Handle_CreatesShipment_ShipsStock_AndMarksOrderShipped()
    {
        // Arrange
        var skuA = new Sku("ABC-123");
        var skuB = new Sku("XYZ-999");

        var order = Order.Create(customerId: 1);
        order.AddLine(skuA, new Quantity(3));
        order.AddLine(skuB, new Quantity(2));
        order.Place();
        order.MarkReserved(); // simulate Step 3 success

        var wh = new Warehouse(1, "Main");
        wh.AddStock(skuA, new Quantity(10));
        wh.AddStock(skuB, new Quantity(10));
        wh.ReserveStock(orderId: 42, sku: skuA, qty: new Quantity(3));
        wh.ReserveStock(orderId: 42, sku: skuB, qty: new Quantity(2));

        var ordersRepo = new InMemoryOrderRepository();
        ordersRepo.Seed(42, order);

        var whRepo = new InMemoryWarehouseRepository();
        whRepo.Seed(wh);

        var shipRepo = new InMemoryShipmentRepository();

        var handler = new CreateShipmentForOrderHandler(ordersRepo, whRepo, shipRepo);

        // Act
        var shipment = await handler.Handle(new CreateShipmentForOrder(OrderId: 42, WarehouseId: 1), now: DateTimeOffset.Parse("2025-01-01T10:00:00Z"));

        // Assert - shipment
        Assert.Equal(42, shipment.OrderId);
        Assert.Equal(1, shipment.WarehouseId);
        Assert.Equal(2, shipment.Lines.Count);

        // Assert - order
        Assert.Equal(OrderStatus.Shipped, order.Status);

        // Assert - warehouse stock updated
        var a = wh.GetOrCreateStockItem(skuA);
        var b = wh.GetOrCreateStockItem(skuB);

        Assert.Equal(7, a.OnHand.Value);
        Assert.Equal(0, a.Reserved.Value);

        Assert.Equal(8, b.OnHand.Value);
        Assert.Equal(0, b.Reserved.Value);

        // Assert - saved
        Assert.Single(shipRepo.Saved);
    }

    [Fact]
    public async Task Handle_WhenOrderNotReserved_Throws()
    {
        // Arrange
        var sku = new Sku("ABC-123");

        var order = Order.Create(customerId: 1);
        order.AddLine(sku, new Quantity(1));
        order.Place(); // NOT reserved

        var wh = new Warehouse(1, "Main");
        wh.AddStock(sku, new Quantity(10));

        var ordersRepo = new InMemoryOrderRepository();
        ordersRepo.Seed(42, order);

        var whRepo = new InMemoryWarehouseRepository();
        whRepo.Seed(wh);

        var shipRepo = new InMemoryShipmentRepository();
        var handler = new CreateShipmentForOrderHandler(ordersRepo, whRepo, shipRepo);

        // Act + Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.Handle(new CreateShipmentForOrder(OrderId: 42, WarehouseId: 1)));

        Assert.Equal(OrderStatus.Placed, order.Status);
        Assert.Empty(shipRepo.Saved);
    }
}
