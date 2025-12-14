using Inventory.Domain.Orders;
using Inventory.Domain.Products;
using Inventory.Domain.Warehouses;
using Xunit;

namespace Inventory.Tests.Orders;

public class OrderTests
{
    [Fact]
    public void AddLine_SameSku_MergesQuantity()
    {
        var order = Order.Create(customerId: 1);

        order.AddLine(new Sku("ABC-123"), new Quantity(2));
        order.AddLine(new Sku("abc-123"), new Quantity(1)); // case-insensitive due to normalization

        Assert.Single(order.Lines);
        Assert.Equal(3, order.Lines[0].Quantity.Value);
    }

    [Fact]
    public void Place_WithZeroLines_Throws()
    {
        var order = Order.Create(customerId: 1);

        var ex = Assert.Throws<InvalidOperationException>(() => order.Place());
        Assert.Contains("zero lines", ex.Message, StringComparison.OrdinalIgnoreCase);
    }
}
