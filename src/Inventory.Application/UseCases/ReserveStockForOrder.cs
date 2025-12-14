namespace Inventory.Application.UseCases;

public sealed record ReserveStockForOrder(int OrderId, int WarehouseId);
