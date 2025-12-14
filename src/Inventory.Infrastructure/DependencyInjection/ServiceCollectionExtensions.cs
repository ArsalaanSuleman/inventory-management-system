

using Inventory.Application.Ports;
using Inventory.Infrastructure.Persistence;
using Inventory.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Inventory.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInventoryInfrastructure(
        this IServiceCollection services,
        IConfiguration config)
    {
        var cs = config.GetConnectionString("InventoryDb")
                 ?? throw new InvalidOperationException("Missing connection string: ConnectionStrings:InventoryDb");

        services.AddDbContext<InventoryDbContext>(opt =>
            opt.UseNpgsql(cs));

        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IWarehouseRepository, WarehouseRepository>();
        services.AddScoped<IShipmentRepository, ShipmentRepository>();

        return services;
    }
}
