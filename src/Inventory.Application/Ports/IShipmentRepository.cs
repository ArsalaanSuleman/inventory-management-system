using Inventory.Domain.Shipping;

namespace Inventory.Application.Ports;

public interface IShipmentRepository
{
    Task SaveAsync(Shipment shipment, CancellationToken ct = default);
}
