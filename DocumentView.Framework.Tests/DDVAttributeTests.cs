using DocumentView.Framework;
using Xunit;

namespace DocumentView.Framework.Tests;

public class DDVMinMaxAttributeTests
{
    [Theory]
    [InlineData(0, 10, 5,  true)]
    [InlineData(0, 10, 0,  true)]   // lower bound
    [InlineData(0, 10, 10, true)]   // upper bound
    [InlineData(0, 10, -1, false)]  // below min
    [InlineData(0, 10, 11, false)]  // above max
    public void Validate_IntValue_BoundaryCheck(double min, double max, int value, bool expected)
    {
        var attr = new DDVMinMaxAttribute(min, max);

        Assert.Equal(expected, attr.Validate(value, out _));
    }

    [Theory]
    [InlineData(0.0, 1.0, 0.5,  true)]
    [InlineData(0.0, 1.0, 0.0,  true)]
    [InlineData(0.0, 1.0, 1.0,  true)]
    [InlineData(0.0, 1.0, 1.01, false)]
    public void Validate_DoubleValue_BoundaryCheck(double min, double max, double value, bool expected)
    {
        var attr = new DDVMinMaxAttribute(min, max);

        Assert.Equal(expected, attr.Validate(value, out _));
    }

    [Fact]
    public void Validate_Failure_ErrorMessage_ContainsRange()
    {
        var attr = new DDVMinMaxAttribute(0, 100);
        attr.Validate(200, out var msg);

        Assert.Contains("0", msg);
        Assert.Contains("100", msg);
    }

    [Fact]
    public void Validate_Failure_CustomMessage_OverridesDefault()
    {
        var attr = new DDVMinMaxAttribute(0, 10) { Message = "Custom error" };
        attr.Validate(99, out var msg);

        Assert.Equal("Custom error", msg);
    }

    [Fact]
    public void Validate_Success_ErrorMessage_IsEmpty()
    {
        var attr = new DDVMinMaxAttribute(0, 10);
        attr.Validate(5, out var msg);

        Assert.Equal(string.Empty, msg);
    }

    [Fact]
    public void Validate_NullValue_ReturnsTrueAndSkipsValidation()
    {
        // null cannot convert → skip (returns true)
        var attr = new DDVMinMaxAttribute(0, 10);

        Assert.True(attr.Validate(null, out _));
    }

    [Fact]
    public void Validate_StringRepresentingNumber_ParsedCorrectly()
    {
        var attr = new DDVMinMaxAttribute(0, 10);

        Assert.True(attr.Validate("5", out _));
        Assert.False(attr.Validate("20", out _));
    }

    [Fact]
    public void Validate_NonConvertibleValue_ReturnsTrueAndSkipsValidation()
    {
        // object cannot convert to double → catch path skips validation
        var attr = new DDVMinMaxAttribute(0, 10);

        Assert.True(attr.Validate(new object(), out var msg));
        Assert.Equal(string.Empty, msg);
    }

    [Fact]
    public void Validate_Failure_ErrorMessage_ContainsDecimalRange()
    {
        // Exercise FormatNumber decimal branch (v != Math.Floor(v))
        var attr = new DDVMinMaxAttribute(0.5, 1.5);
        attr.Validate(2.0, out var msg);

        Assert.Contains("0.5", msg);
        Assert.Contains("1.5", msg);
    }
}

public class DDVMaxCharsAttributeTests
{
    [Theory]
    [InlineData(10, "hello",      true)]   // 5 chars ≤ 10
    [InlineData(5,  "hello",      true)]   // boundary
    [InlineData(4,  "hello",      false)]  // 5 > 4
    [InlineData(0,  "",           true)]   // empty ≤ 0
    [InlineData(0,  "x",          false)]  // 1 > 0
    public void Validate_StringLength_BoundaryCheck(int maxLength, string value, bool expected)
    {
        var attr = new DDVMaxCharsAttribute(maxLength);

        Assert.Equal(expected, attr.Validate(value, out _));
    }

    [Fact]
    public void Validate_NullValue_TreatedAsEmpty_ReturnsTrue()
    {
        var attr = new DDVMaxCharsAttribute(10);

        Assert.True(attr.Validate(null, out _));
    }

    [Fact]
    public void Validate_Failure_ErrorMessage_ContainsMaxLength()
    {
        var attr = new DDVMaxCharsAttribute(5);
        attr.Validate("toolong!", out var msg);

        Assert.Contains("5", msg);
    }

    [Fact]
    public void Validate_Failure_CustomMessage_OverridesDefault()
    {
        var attr = new DDVMaxCharsAttribute(3) { Message = "Too long" };
        attr.Validate("tooolong", out var msg);

        Assert.Equal("Too long", msg);
    }

    [Fact]
    public void Validate_Success_ErrorMessage_IsEmpty()
    {
        var attr = new DDVMaxCharsAttribute(10);
        attr.Validate("ok", out var msg);

        Assert.Equal(string.Empty, msg);
    }
}

public class DDXAttributeTests
{
    [Fact]
    public void ControlName_IsSetFromConstructor()
    {
        var attr = new DDXAttribute("IDC_EDIT_NAME");

        Assert.Equal("IDC_EDIT_NAME", attr.ControlName);
    }

    [Fact]
    public void ControlProperty_DefaultsToNull()
    {
        var attr = new DDXAttribute("IDC_EDIT_NAME");

        Assert.Null(attr.ControlProperty);
    }

    [Fact]
    public void ControlProperty_CanBeSet()
    {
        var attr = new DDXAttribute("IDC_COMBO") { ControlProperty = "SelectedIndex" };

        Assert.Equal("SelectedIndex", attr.ControlProperty);
    }
}
