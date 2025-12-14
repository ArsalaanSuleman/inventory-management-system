using Inventory.Application.Ports;
using Inventory.Domain.Warehouses;

namespace Inventory.Tests.TestDoubles;

public sealed class InMemoryWarehouseRepository : IWarehouseRepository
{
    private readonly Dictionary<int, Warehouse> _store = new();

    public Task<Warehouse?> GetByIdAsync(int id, CancellationToken ct = default)
        => Task.FromResult(_store.TryGetValue(id, out var w) ? w : null);

    public Task SaveAsync(Warehouse warehouse, CancellationToken ct = default)
    {
        _store[warehouse.Id] = warehouse;
        return Task.CompletedTask;
    }

    public void Seed(Warehouse warehouse) => _store[warehouse.Id] = warehouse;
}
