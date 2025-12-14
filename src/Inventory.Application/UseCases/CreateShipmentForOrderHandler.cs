using Inventory.Application.Ports;
using Inventory.Domain.Orders;
using Inventory.Domain.Shipping;

namespace Inventory.Application.UseCases;

public sealed class CreateShipmentForOrderHandler
{
    private readonly IOrderRepository _orders;
    private readonly IWarehouseRepository _warehouses;
    private readonly IShipmentRepository _shipments;

    public CreateShipmentForOrderHandler(
        IOrderRepository orders,
        IWarehouseRepository warehouses,
        IShipmentRepository shipments)
    {
        _orders = orders;
        _warehouses = warehouses;
        _shipments = shipments;
    }

    public async Task<Shipment> Handle(CreateShipmentForOrder cmd, DateTimeOffset? now = null, CancellationToken ct = default)
    {
        var order = await _orders.GetByIdAsync(cmd.OrderId, ct)
            ?? throw new InvalidOperationException($"Order {cmd.OrderId} not found.");

        var warehouse = await _warehouses.GetByIdAsync(cmd.WarehouseId, ct)
            ?? throw new InvalidOperationException($"Warehouse {cmd.WarehouseId} not found.");

        if (order.Status != OrderStatus.Reserved)
            throw new InvalidOperationException($"Order must be Reserved to ship (current: {order.Status}).");

        // Build shipment from order lines
        var shipment = Shipment.FromOrder(
            orderId: cmd.OrderId,
            warehouseId: cmd.WarehouseId,
            lines: order.Lines.Select(l => (l.Sku, l.Quantity)),
            createdAt: now ?? DateTimeOffset.UtcNow
        );

        // Apply stock changes (must not exceed reserved)
        foreach (var line in shipment.Lines)
        {
            warehouse.ShipReservedStock(line.Sku, line.Quantity);
        }

        order.MarkShipped();

        await _shipments.SaveAsync(shipment, ct);
        await _warehouses.SaveAsync(warehouse, ct);
        await _orders.SaveAsync(order, ct);

        return shipment;
    }
}
