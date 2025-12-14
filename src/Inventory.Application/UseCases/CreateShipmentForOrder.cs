namespace Inventory.Application.UseCases;

public sealed record CreateShipmentForOrder(int OrderId, int WarehouseId);
