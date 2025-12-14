using Inventory.Application.UseCases;
using Inventory.Domain.Orders;
using Inventory.Domain.Products;
using Inventory.Domain.Warehouses;
using Inventory.Tests.TestDoubles;
using Xunit;

namespace Inventory.Tests.UseCases;

public class ReserveStockForOrderTests
{
    [Fact]
    public async Task Handle_ReservesStock_AndMarksOrderReserved()
    {
        // Arrange
        var order = Order.Create(customerId: 1);
        order.AddLine(new Sku("ABC-123"), new Quantity(3));
        order.AddLine(new Sku("XYZ-999"), new Quantity(2));
        order.Place();

        var wh = new Warehouse(1, "Main");
        wh.AddStock(new Sku("ABC-123"), new Quantity(10));
        wh.AddStock(new Sku("XYZ-999"), new Quantity(10));

        var ordersRepo = new InMemoryOrderRepository();
        ordersRepo.Seed(42, order);

        var whRepo = new InMemoryWarehouseRepository();
        whRepo.Seed(wh);

        var handler = new ReserveStockForOrderHandler(ordersRepo, whRepo);

        // Act
        await handler.Handle(new ReserveStockForOrder(OrderId: 42, WarehouseId: 1));

        // Assert
        Assert.Equal(OrderStatus.Reserved, order.Status);

        var itemA = wh.GetOrCreateStockItem(new Sku("ABC-123"));
        var itemB = wh.GetOrCreateStockItem(new Sku("XYZ-999"));

        Assert.Equal(3, itemA.Reserved.Value);
        Assert.Equal(7, itemA.Available.Value);

        Assert.Equal(2, itemB.Reserved.Value);
        Assert.Equal(8, itemB.Available.Value);
    }

    [Fact]
    public async Task Handle_WhenNotEnoughStock_Throws_AndOrderNotReserved()
    {
        // Arrange
        var order = Order.Create(customerId: 1);
        order.AddLine(new Sku("ABC-123"), new Quantity(6));
        order.Place();

        var wh = new Warehouse(1, "Main");
        wh.AddStock(new Sku("ABC-123"), new Quantity(5));

        var ordersRepo = new InMemoryOrderRepository();
        ordersRepo.Seed(42, order);

        var whRepo = new InMemoryWarehouseRepository();
        whRepo.Seed(wh);

        var handler = new ReserveStockForOrderHandler(ordersRepo, whRepo);

        // Act + Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.Handle(new ReserveStockForOrder(OrderId: 42, WarehouseId: 1)));

        Assert.Equal(OrderStatus.Placed, order.Status);
        Assert.Equal(0, wh.GetOrCreateStockItem(new Sku("ABC-123")).Reserved.Value);
    }
}
