using DocumentView.Framework;
using Xunit;

namespace DocumentView.Framework.Tests;

public class ControlValueConverterTests
{
    // ── TextBox ──────────────────────────────────────────────────────────────

    [StaFact]
    public void GetValue_TextBox_AsString()
    {
        var tb = new TextBox { Text = "hello" };

        Assert.Equal("hello", ControlValueConverter.GetValue(tb, typeof(string)));
    }

    [StaFact]
    public void GetValue_TextBox_AsInt_ParsesText()
    {
        var tb = new TextBox { Text = "42" };

        Assert.Equal(42, ControlValueConverter.GetValue(tb, typeof(int)));
    }

    [StaFact]
    public void GetValue_TextBox_AsDouble_ParsesText()
    {
        var tb = new TextBox { Text = "3.14" };

        Assert.Equal(3.14, ControlValueConverter.GetValue(tb, typeof(double)));
    }

    [StaFact]
    public void SetValue_TextBox_SetsText()
    {
        var tb = new TextBox();
        ControlValueConverter.SetValue(tb, "world");

        Assert.Equal("world", tb.Text);
    }

    [StaFact]
    public void SetValue_TextBox_IntValue_ConvertedToString()
    {
        var tb = new TextBox();
        ControlValueConverter.SetValue(tb, 123);

        Assert.Equal("123", tb.Text);
    }

    // ── CheckBox ─────────────────────────────────────────────────────────────

    [StaFact]
    public void GetValue_CheckBox_AsBool_ReturnsChecked()
    {
        var cb = new CheckBox { Checked = true };

        Assert.Equal(true, ControlValueConverter.GetValue(cb, typeof(bool)));
    }

    [StaFact]
    public void SetValue_CheckBox_SetsBool()
    {
        var cb = new CheckBox();
        ControlValueConverter.SetValue(cb, true);

        Assert.True(cb.Checked);
    }

    // ── NumericUpDown ─────────────────────────────────────────────────────────

    [StaFact]
    public void GetValue_NumericUpDown_AsInt_ReturnsValue()
    {
        var nud = new NumericUpDown { Minimum = 0, Maximum = 100, Value = 55 };

        Assert.Equal(55, ControlValueConverter.GetValue(nud, typeof(int)));
    }

    [StaFact]
    public void SetValue_NumericUpDown_SetsValue()
    {
        var nud = new NumericUpDown { Minimum = 0, Maximum = 100 };
        ControlValueConverter.SetValue(nud, 75);

        Assert.Equal(75m, nud.Value);
    }

    [StaFact]
    public void SetValue_NumericUpDown_ClampsToMaximum()
    {
        var nud = new NumericUpDown { Minimum = 0, Maximum = 100 };
        ControlValueConverter.SetValue(nud, 200);

        Assert.Equal(100m, nud.Value);
    }

    [StaFact]
    public void SetValue_NumericUpDown_ClampsToMinimum()
    {
        var nud = new NumericUpDown { Minimum = 0, Maximum = 100 };
        ControlValueConverter.SetValue(nud, -50);

        Assert.Equal(0m, nud.Value);
    }

    // ── null ─────────────────────────────────────────────────────────────────

    [StaFact]
    public void SetValue_Null_DoesNotChangeControl()
    {
        var tb = new TextBox { Text = "original" };
        ControlValueConverter.SetValue(tb, null);

        Assert.Equal("original", tb.Text);
    }

    // ── ComboBox ─────────────────────────────────────────────────────────────

    [StaFact]
    public void GetValue_ComboBox_IntType_ReturnsSelectedIndex()
    {
        var cmb = new ComboBox();
        cmb.Items.AddRange(["A", "B", "C"]);
        cmb.SelectedIndex = 1;

        Assert.Equal(1, ControlValueConverter.GetValue(cmb, typeof(int)));
    }

    [StaFact]
    public void SetValue_ComboBox_IntValue_SetsSelectedIndex()
    {
        var cmb = new ComboBox();
        cmb.Items.AddRange(["A", "B", "C"]);
        ControlValueConverter.SetValue(cmb, 2);

        Assert.Equal(2, cmb.SelectedIndex);
    }

    // ── RadioButton ──────────────────────────────────────────────────────────

    [StaFact]
    public void GetValue_RadioButton_AsBool_ReturnsChecked()
    {
        var rb = new RadioButton { Checked = true };

        Assert.Equal(true, ControlValueConverter.GetValue(rb, typeof(bool)));
    }

    [StaFact]
    public void SetValue_RadioButton_SetsBool()
    {
        var rb = new RadioButton();
        ControlValueConverter.SetValue(rb, true);

        Assert.True(rb.Checked);
    }

    // ── Label ────────────────────────────────────────────────────────────────

    [StaFact]
    public void SetValue_Label_SetsText()
    {
        var lbl = new Label();
        ControlValueConverter.SetValue(lbl, "status");

        Assert.Equal("status", lbl.Text);
    }

    [StaFact]
    public void GetValue_Label_ReturnsText()
    {
        var lbl = new Label { Text = "status" };

        Assert.Equal("status", ControlValueConverter.GetValue(lbl, typeof(string)));
    }

    // ── ComboBox Text path ───────────────────────────────────────────────────

    [StaFact]
    public void GetValue_ComboBox_StringType_ReturnsText()
    {
        var cmb = new ComboBox { Text = "selected" };

        Assert.Equal("selected", ControlValueConverter.GetValue(cmb, typeof(string)));
    }

    [StaFact]
    public void SetValue_ComboBox_StringValue_SetsText()
    {
        var cmb = new ComboBox();
        ControlValueConverter.SetValue(cmb, "option");

        Assert.Equal("option", cmb.Text);
    }

    // ── ListBox ──────────────────────────────────────────────────────────────

    [StaFact]
    public void GetValue_ListBox_IntType_ReturnsSelectedIndex()
    {
        var lb = new ListBox();
        lb.Items.AddRange(["A", "B", "C"]);
        lb.SelectedIndex = 2;

        Assert.Equal(2, ControlValueConverter.GetValue(lb, typeof(int)));
    }

    [StaFact]
    public void GetValue_ListBox_StringType_ReturnsText()
    {
        var lb = new ListBox();
        lb.Items.AddRange(["A", "B", "C"]);
        lb.SelectedIndex = 1;

        Assert.Equal("B", ControlValueConverter.GetValue(lb, typeof(string)));
    }

    [StaFact]
    public void SetValue_ListBox_IntValue_SetsSelectedIndex()
    {
        var lb = new ListBox();
        lb.Items.AddRange(["A", "B", "C"]);
        ControlValueConverter.SetValue(lb, 1);

        Assert.Equal(1, lb.SelectedIndex);
    }

    [StaFact]
    public void SetValue_ListBox_StringValue_SetsText()
    {
        var lb = new ListBox();
        ControlValueConverter.SetValue(lb, "item");

        Assert.Equal("item", lb.Text);
    }

    // ── DataGridView ─────────────────────────────────────────────────────────

    [StaFact]
    public void GetValue_DataGridView_ReturnsDataSource()
    {
        var source = new System.ComponentModel.BindingList<string>();
        var dgv = new DataGridView { DataSource = source };

        Assert.Same(source, ControlValueConverter.GetValue(dgv, typeof(object)));
    }

    [StaFact]
    public void SetValue_DataGridView_NewSource_SetsDataSource()
    {
        var dgv = new DataGridView();
        var source = new System.ComponentModel.BindingList<string>();
        ControlValueConverter.SetValue(dgv, source);

        Assert.Same(source, dgv.DataSource);
    }

    [StaFact]
    public void SetValue_DataGridView_SameReference_DoesNotRebind()
    {
        var source = new System.ComponentModel.BindingList<string>();
        var dgv = new DataGridView { DataSource = source };
        ControlValueConverter.SetValue(dgv, source);  // same reference: no rebind

        Assert.Same(source, dgv.DataSource);
    }

    // ── Unknown control ─────────────────────────────────────────────────────

    [StaFact]
    public void GetValue_UnknownControl_ReturnsNull()
    {
        var btn = new Button();

        Assert.Null(ControlValueConverter.GetValue(btn, typeof(string)));
    }

    // ── ConvertTo: string → decimal / bool ───────────────────────────────────

    [StaFact]
    public void GetValue_TextBox_AsDecimal_ParsesText()
    {
        var tb = new TextBox { Text = "1.23" };

        Assert.Equal(1.23m, ControlValueConverter.GetValue(tb, typeof(decimal)));
    }

    [StaFact]
    public void GetValue_TextBox_AsBool_ParsesText()
    {
        var tb = new TextBox { Text = "true" };

        Assert.Equal(true, ControlValueConverter.GetValue(tb, typeof(bool)));
    }

    // ── ToBool: int / string / other ─────────────────────────────────────────

    [StaFact]
    public void SetValue_CheckBox_IntNonZero_SetsCheckedTrue()
    {
        var cb = new CheckBox();
        ControlValueConverter.SetValue(cb, 1);

        Assert.True(cb.Checked);
    }

    [StaFact]
    public void SetValue_CheckBox_IntZero_SetsCheckedFalse()
    {
        var cb = new CheckBox { Checked = true };
        ControlValueConverter.SetValue(cb, 0);

        Assert.False(cb.Checked);
    }

    [StaFact]
    public void SetValue_CheckBox_StringTrue_SetsCheckedTrue()
    {
        var cb = new CheckBox();
        ControlValueConverter.SetValue(cb, "true");

        Assert.True(cb.Checked);
    }

    [StaFact]
    public void SetValue_CheckBox_DoubleNonZero_SetsCheckedTrue()
    {
        var cb = new CheckBox();
        ControlValueConverter.SetValue(cb, 1.0);  // Convert.ToBoolean(double) path

        Assert.True(cb.Checked);
    }
}
