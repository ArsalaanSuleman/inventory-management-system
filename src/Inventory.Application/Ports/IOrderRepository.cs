using Inventory.Domain.Orders;

namespace Inventory.Application.Ports;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(int id, CancellationToken ct = default);
    Task SaveAsync(Order order, CancellationToken ct = default);
}
