using Inventory.Domain.Products;
using Inventory.Domain.Warehouses;
using Inventory.Domain.Shipping;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Infrastructure.Persistence;

internal sealed class ShipmentConfiguration : IEntityTypeConfiguration<Shipment>
{
    public void Configure(EntityTypeBuilder<Shipment> b)
    {
        b.ToTable("Shipments");
        b.HasKey(x => x.Id);

        b.Property(x => x.OrderId).IsRequired();
        b.Property(x => x.WarehouseId).IsRequired();
        b.Property(x => x.CreatedAt).IsRequired();

        b.OwnsMany(typeof(ShipmentLine), "Lines", lines =>
        {
            lines.ToTable("ShipmentLines");
            lines.WithOwner().HasForeignKey("ShipmentId");
            lines.HasKey("ShipmentId", "Id");

            lines.Property<int>("Id").ValueGeneratedNever();

            lines.Property<Sku>("Sku")
                .HasConversion(
                    v => v.ToString(),
                    v => new Sku(v))
                .HasMaxLength(64)
                .IsRequired();

            lines.Property<Quantity>("Quantity")
                .HasConversion(
                    v => v.Value,
                    v => new Quantity(v))
                .IsRequired();
        });

        b.Navigation("Lines").Metadata.SetField("_lines");
        b.Navigation("Lines").UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
