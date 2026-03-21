using DocumentView.Sample;
using Xunit;

namespace DocumentView.Sample.Tests;

public class ItemRowTests
{
    [Fact]
    public void Properties_SetAndGet()
    {
        var row = new ItemRow { No = 5, Name = "Test", Active = true };

        Assert.Equal(5, row.No);
        Assert.Equal("Test", row.Name);
        Assert.True(row.Active);
    }

    [Fact]
    public void PropertyChanged_FiredFor_No()
    {
        var row = new ItemRow();
        var fired = new List<string?>();
        row.PropertyChanged += (_, e) => fired.Add(e.PropertyName);

        row.No = 10;

        Assert.Contains(nameof(ItemRow.No), fired);
    }

    [Fact]
    public void PropertyChanged_FiredFor_Name()
    {
        var row = new ItemRow();
        var fired = new List<string?>();
        row.PropertyChanged += (_, e) => fired.Add(e.PropertyName);

        row.Name = "New name";

        Assert.Contains(nameof(ItemRow.Name), fired);
    }

    [Fact]
    public void PropertyChanged_FiredFor_Active()
    {
        var row = new ItemRow();
        var fired = new List<string?>();
        row.PropertyChanged += (_, e) => fired.Add(e.PropertyName);

        row.Active = false;

        Assert.Contains(nameof(ItemRow.Active), fired);
    }

    [Fact]
    public void PropertyChanged_FiredForAllThreeProperties()
    {
        var row = new ItemRow();
        var fired = new List<string?>();
        row.PropertyChanged += (_, e) => fired.Add(e.PropertyName);

        row.No     = 1;
        row.Name   = "item";
        row.Active = true;

        Assert.Equal(3, fired.Count);
    }

    [Fact]
    public void DefaultValues_AreCorrect()
    {
        var row = new ItemRow();

        Assert.Equal(0, row.No);
        Assert.Equal(string.Empty, row.Name);
        Assert.True(row.Active); // default true
    }
}
