

using System.Text.Json;
using Inventory.Domain.Common;
using Inventory.Domain.Orders;
using Inventory.Domain.Shipping;
using Inventory.Domain.Warehouses;
using Inventory.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence;

public sealed class InventoryDbContext : DbContext
{
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options) { }

    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    public DbSet<Shipment> Shipments => Set<Shipment>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new OrderConfiguration());
        modelBuilder.ApplyConfiguration(new WarehouseConfiguration());
        modelBuilder.ApplyConfiguration(new ShipmentConfiguration());

        modelBuilder.Entity<OutboxMessage>(b =>
        {
            b.ToTable("OutboxMessages");
            b.HasKey(x => x.Id);
            b.Property(x => x.Type).IsRequired().HasMaxLength(200);
            b.Property(x => x.PayloadJson).IsRequired();
            b.Property(x => x.OccurredAt).IsRequired();
            b.Property(x => x.ProcessedAt);
            b.Property(x => x.Error);
            b.HasIndex(x => x.ProcessedAt);
        });
    }

    public override int SaveChanges()
    {
        AddDomainEventsToOutbox();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        AddDomainEventsToOutbox();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void AddDomainEventsToOutbox()
    {
        var aggregates = ChangeTracker.Entries()
            .Where(e => e.Entity is AggregateRoot)
            .Select(e => (AggregateRoot)e.Entity)
            .ToList();

        var events = aggregates
            .SelectMany(a => a.DomainEvents)
            .ToList();

        if (events.Count == 0) return;

        foreach (var ev in events)
        {
            OutboxMessages.Add(new OutboxMessage
            {
                Id = ev.EventId,
                OccurredAt = ev.OccurredAt,
                Type = ev.EventType,
                PayloadJson = JsonSerializer.Serialize(ev, ev.GetType(), JsonOptions)
            });
        }

        foreach (var a in aggregates)
            a.ClearDomainEvents();
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = false
    };
}
