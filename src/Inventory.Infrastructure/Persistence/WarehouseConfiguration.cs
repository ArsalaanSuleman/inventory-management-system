using Inventory.Domain.Products;
using Inventory.Domain.Warehouses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Infrastructure.Persistence;

internal sealed class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
{
    public void Configure(EntityTypeBuilder<Warehouse> b)
    {
        b.ToTable("Warehouses");
        b.HasKey(x => x.Id);

        b.Property(x => x.Name).HasMaxLength(200).IsRequired();

        b.OwnsMany(typeof(StockItem), "Stock", stock =>
        {
            stock.ToTable("StockItems");
            stock.WithOwner().HasForeignKey("WarehouseId");

            stock.HasKey("WarehouseId", "Sku");

            stock.Property<Sku>("Sku")
                .HasConversion(
                    v => v.ToString(),
                    v => new Sku(v))
                .HasMaxLength(64)
                .IsRequired();

            stock.Property<Quantity>("OnHand")
                .HasConversion(v => v.Value, v => new Quantity(v))
                .IsRequired();

            stock.Property<Quantity>("Reserved")
                .HasConversion(v => v.Value, v => new Quantity(v))
                .IsRequired();
        });
    }
}
