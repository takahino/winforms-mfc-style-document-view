using DocumentView.Sample2;
using Xunit;

namespace DocumentView.Sample2.Tests;

public class OrderLineTests
{
    // ── Initial values ────────────────────────────────────────────────────

    [Fact]
    public void InitialValues_AreCorrect()
    {
        var line = new OrderLine();

        Assert.Equal(string.Empty, line.ItemCode);
        Assert.Equal(string.Empty, line.ItemName);
        Assert.Equal(1,  line.Qty);
        Assert.Equal(0m, line.UnitPrice);
        Assert.Equal(0m, line.Amount);
    }

    // ── Amount calculation ────────────────────────────────────────────────

    [Fact]
    public void Amount_IsQtyTimesUnitPrice()
    {
        var line = new OrderLine { Qty = 3, UnitPrice = 500m };
        Assert.Equal(1500m, line.Amount);
    }

    [Fact]
    public void Amount_WithZeroQty_IsZero()
    {
        var line = new OrderLine { Qty = 0, UnitPrice = 1000m };
        Assert.Equal(0m, line.Amount);
    }

    [Fact]
    public void Amount_WithZeroUnitPrice_IsZero()
    {
        var line = new OrderLine { Qty = 5, UnitPrice = 0m };
        Assert.Equal(0m, line.Amount);
    }

    [Fact]
    public void Amount_WithDecimalUnitPrice_CalculatesCorrectly()
    {
        var line = new OrderLine { Qty = 2, UnitPrice = 123.45m };
        Assert.Equal(246.90m, line.Amount);
    }

    // ── PropertyChanged — Qty ─────────────────────────────────────────────

    [Fact]
    public void Qty_Setter_RaisesPropertyChangedForQty()
    {
        var line     = new OrderLine();
        var changed  = new List<string?>();
        line.PropertyChanged += (_, e) => changed.Add(e.PropertyName);

        line.Qty = 5;

        Assert.Contains(nameof(OrderLine.Qty), changed);
    }

    [Fact]
    public void Qty_Setter_RaisesPropertyChangedForAmount()
    {
        var line     = new OrderLine();
        var changed  = new List<string?>();
        line.PropertyChanged += (_, e) => changed.Add(e.PropertyName);

        line.Qty = 5;

        Assert.Contains(nameof(OrderLine.Amount), changed);
    }

    // ── PropertyChanged — UnitPrice ───────────────────────────────────────

    [Fact]
    public void UnitPrice_Setter_RaisesPropertyChangedForUnitPrice()
    {
        var line    = new OrderLine();
        var changed = new List<string?>();
        line.PropertyChanged += (_, e) => changed.Add(e.PropertyName);

        line.UnitPrice = 999m;

        Assert.Contains(nameof(OrderLine.UnitPrice), changed);
    }

    [Fact]
    public void UnitPrice_Setter_RaisesPropertyChangedForAmount()
    {
        var line    = new OrderLine();
        var changed = new List<string?>();
        line.PropertyChanged += (_, e) => changed.Add(e.PropertyName);

        line.UnitPrice = 999m;

        Assert.Contains(nameof(OrderLine.Amount), changed);
    }

    // ── PropertyChanged — ItemCode / ItemName ─────────────────────────────

    [Fact]
    public void ItemCode_Setter_RaisesPropertyChanged()
    {
        var line    = new OrderLine();
        var changed = new List<string?>();
        line.PropertyChanged += (_, e) => changed.Add(e.PropertyName);

        line.ItemCode = "ITEM-001";

        Assert.Contains(nameof(OrderLine.ItemCode), changed);
    }

    [Fact]
    public void ItemName_Setter_RaisesPropertyChanged()
    {
        var line    = new OrderLine();
        var changed = new List<string?>();
        line.PropertyChanged += (_, e) => changed.Add(e.PropertyName);

        line.ItemName = "Widget A";

        Assert.Contains(nameof(OrderLine.ItemName), changed);
    }

    // ── PropertyChanged sender ────────────────────────────────────────────

    [Fact]
    public void PropertyChanged_FiresWithCorrectSender()
    {
        var line   = new OrderLine();
        object? sender = null;
        line.PropertyChanged += (s, _) => sender = s;

        line.Qty = 10;

        Assert.Same(line, sender);
    }
}
