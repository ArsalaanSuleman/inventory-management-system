using Inventory.Application.Ports;
using Inventory.Domain.Warehouses;
using Inventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Repositories;

public sealed class WarehouseRepository : IWarehouseRepository
{
    private readonly InventoryDbContext _db;

    public WarehouseRepository(InventoryDbContext db) => _db = db;

    public Task<Warehouse?> GetByIdAsync(int id, CancellationToken ct = default)
        => _db.Warehouses.FirstOrDefaultAsync(w => w.Id == id, ct);

    public Task SaveAsync(Warehouse warehouse, CancellationToken ct = default)
    {
        if (_db.Entry(warehouse).State == EntityState.Detached)
            _db.Warehouses.Add(warehouse);

        return _db.SaveChangesAsync(ct);
    }
}
