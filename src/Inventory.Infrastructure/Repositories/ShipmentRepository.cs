using Inventory.Application.Ports;
using Inventory.Domain.Shipping;
using Inventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Repositories;

public sealed class ShipmentRepository : IShipmentRepository
{
    private readonly InventoryDbContext _db;

    public ShipmentRepository(InventoryDbContext db) => _db = db;

    public Task SaveAsync(Shipment shipment, CancellationToken ct = default)
    {
        if (_db.Entry(shipment).State == EntityState.Detached)
            _db.Shipments.Add(shipment);

        return _db.SaveChangesAsync(ct);
    }
}
