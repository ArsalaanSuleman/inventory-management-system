using Inventory.Application.Ports;
using Inventory.Domain.Shipping;

namespace Inventory.Tests.TestDoubles;

public sealed class InMemoryShipmentRepository : IShipmentRepository
{
    public readonly List<Shipment> Saved = new();

    public Task SaveAsync(Shipment shipment, CancellationToken ct = default)
    {
        Saved.Add(shipment);
        return Task.CompletedTask;
    }
}
