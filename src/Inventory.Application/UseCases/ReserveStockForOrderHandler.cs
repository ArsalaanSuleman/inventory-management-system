using Inventory.Application.Ports;

namespace Inventory.Application.UseCases;

public sealed class ReserveStockForOrderHandler
{
    private readonly IOrderRepository _orders;
    private readonly IWarehouseRepository _warehouses;

    public ReserveStockForOrderHandler(IOrderRepository orders, IWarehouseRepository warehouses)
    {
        _orders = orders;
        _warehouses = warehouses;
    }

    public async Task Handle(ReserveStockForOrder cmd, CancellationToken ct = default)
    {
        var order = await _orders.GetByIdAsync(cmd.OrderId, ct)
            ?? throw new InvalidOperationException($"Order {cmd.OrderId} not found.");

        var warehouse = await _warehouses.GetByIdAsync(cmd.WarehouseId, ct)
            ?? throw new InvalidOperationException($"Warehouse {cmd.WarehouseId} not found.");

        if (order.Status != Inventory.Domain.Orders.OrderStatus.Placed)
            throw new InvalidOperationException($"Order must be Placed to reserve stock (current: {order.Status}).");

        // Reserve per line. If any reserve fails, it throws and nothing is saved.
        foreach (var line in order.Lines)
        {
            warehouse.ReserveStock(orderId: cmd.OrderId, sku: line.Sku, qty: line.Quantity);

        }

        order.MarkReserved();

        await _warehouses.SaveAsync(warehouse, ct);
        await _orders.SaveAsync(order, ct);
    }
}
