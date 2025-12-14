using Inventory.Application.Ports;
using Inventory.Domain.Orders;
using Inventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Repositories;

public sealed class OrderRepository : IOrderRepository
{
    private readonly InventoryDbContext _db;

    public OrderRepository(InventoryDbContext db) => _db = db;

    public Task<Order?> GetByIdAsync(int id, CancellationToken ct = default)
        => _db.Orders.FirstOrDefaultAsync(o => o.Id == id, ct);

    public Task SaveAsync(Order order, CancellationToken ct = default)
    {
        if (_db.Entry(order).State == EntityState.Detached)
            _db.Orders.Add(order);

        return _db.SaveChangesAsync(ct);
    }
}
