using Inventory.Domain.Products;
using Inventory.Domain.Warehouses;
using Inventory.Domain.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Infrastructure.Persistence;

internal sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> b)
    {
        b.ToTable("Orders");
        b.HasKey(x => x.Id);

        b.Property(x => x.CustomerId).IsRequired();
        b.Property(x => x.Status).IsRequired();

        b.OwnsMany(typeof(OrderLine), "Lines", lines =>
        {
            lines.ToTable("OrderLines");
            lines.WithOwner().HasForeignKey("OrderId");
            lines.HasKey("OrderId", "Id");

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
