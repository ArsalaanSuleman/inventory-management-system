using Inventory.Application.Ports;
using Inventory.Domain.Orders;

namespace Inventory.Tests.TestDoubles;

public sealed class InMemoryOrderRepository : IOrderRepository
{
    private readonly Dictionary<int, Order> _store = new();

    public Task<Order?> GetByIdAsync(int id, CancellationToken ct = default)
        => Task.FromResult(_store.TryGetValue(id, out var o) ? o : null);

    public Task SaveAsync(Order order, CancellationToken ct = default)
    {
        // If Id isn't set yet, assign one.
        if (order.Id == 0)
        {
            var nextId = _store.Count == 0 ? 1 : _store.Keys.Max() + 1;

            // We can’t set Id directly because it’s private set.
            // For now: keep “Id” as a persistence concern later, and store by generated key.
            // We'll store it under nextId and accept that order.Id stays 0 until Infrastructure.
            _store[nextId] = order;
            return Task.CompletedTask;
        }

        _store[order.Id] = order;
        return Task.CompletedTask;
    }

    // Helper for tests: seed with explicit id
    public void Seed(int id, Order order) => _store[id] = order;
}
