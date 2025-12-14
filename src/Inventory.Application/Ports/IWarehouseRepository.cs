using Inventory.Domain.Warehouses;

namespace Inventory.Application.Ports;

public interface IWarehouseRepository
{
    Task<Warehouse?> GetByIdAsync(int id, CancellationToken ct = default);
    Task SaveAsync(Warehouse warehouse, CancellationToken ct = default);
}
