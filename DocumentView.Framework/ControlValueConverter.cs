using System.Globalization;

namespace DocumentView.Framework;

/// <summary>
/// WinForms control ↔ CLR value conversion.
/// To support more control types, add cases to the switches below.
/// </summary>
public static class ControlValueConverter
{
    // UpdateData(true): control → document field
    public static object? GetValue(Control control, Type targetType, string? controlProperty = null)
    {
        try
        {
            return control switch
            {
                TextBox tb      => ConvertTo(tb.Text, targetType),
                CheckBox cb     => ConvertTo(cb.Checked, targetType),
                RadioButton rb  => ConvertTo(rb.Checked, targetType),
                Label lbl       => ConvertTo(lbl.Text, targetType),
                NumericUpDown n => ConvertTo(n.Value, targetType),

                ComboBox cmb => (controlProperty ?? (targetType == typeof(int) ? "SelectedIndex" : "Text")) switch
                {
                    "SelectedIndex" => ConvertTo(cmb.SelectedIndex, targetType),
                    "SelectedItem"  => ConvertTo(cmb.SelectedItem, targetType),
                    _               => ConvertTo(cmb.Text, targetType),
                },

                ListBox lb => (controlProperty ?? (targetType == typeof(int) ? "SelectedIndex" : "Text")) switch
                {
                    "SelectedIndex" => ConvertTo(lb.SelectedIndex, targetType),
                    _               => ConvertTo(lb.Text, targetType),
                },

                // DataGridView: return DataSource (BindingList<T>) as-is.
                // BindingList live-binds grid edits to the document field without UpdateData(true).
                // UpdateData(true) is a no-op when field and DataSource are the same reference.
                DataGridView dgv => dgv.DataSource,

                _ => null,
            };
        }
        catch (FormatException) { return null; }
    }

    // UpdateData(false): document field → control
    public static void SetValue(Control control, object? value, string? controlProperty = null)
    {
        if (value is null) return;
        switch (control)
        {
            case TextBox tb:
                tb.Text = value.ToString() ?? "";
                break;
            case CheckBox cb:
                cb.Checked = ToBool(value);
                break;
            case RadioButton rb:
                rb.Checked = ToBool(value);
                break;
            case Label lbl:
                lbl.Text = value.ToString() ?? "";
                break;
            case NumericUpDown nud:
                var d = Convert.ToDecimal(value, CultureInfo.InvariantCulture);
                nud.Value = Math.Clamp(d, nud.Minimum, nud.Maximum);
                break;
            case ComboBox cmb:
                if ((controlProperty ?? (value is int ? "SelectedIndex" : "Text")) == "SelectedIndex" && value is int si)
                    cmb.SelectedIndex = si;
                else
                    cmb.Text = value.ToString() ?? "";
                break;
            case ListBox lb:
                if ((controlProperty ?? (value is int ? "SelectedIndex" : "Text")) == "SelectedIndex" && value is int lsi)
                    lb.SelectedIndex = lsi;
                else
                    lb.Text = value.ToString() ?? "";
                break;

            // DataGridView: set document BindingList<T> as DataSource.
            // Skip rebind when same reference (preserve selection and scroll).
            case DataGridView dgv:
                if (!ReferenceEquals(dgv.DataSource, value))
                    dgv.DataSource = value;
                break;
        }
    }

    private static object? ConvertTo(object? value, Type targetType)
    {
        if (value is null) return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
        var t = Nullable.GetUnderlyingType(targetType) ?? targetType;
        if (value.GetType() == t) return value;
        if (value is string s)
        {
            if (t == typeof(string)) return s;
            if (t == typeof(int)) return int.TryParse(s, out var i) ? i : 0;
            if (t == typeof(double)) return double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var dv) ? dv : 0.0;
            if (t == typeof(decimal)) return decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var dm) ? dm : 0m;
            if (t == typeof(bool)) return bool.TryParse(s, out var b) ? b : false;
        }
        try { return Convert.ChangeType(value, t, CultureInfo.InvariantCulture); }
        catch { return targetType.IsValueType ? Activator.CreateInstance(targetType) : null; }
    }

    private static bool ToBool(object value) => value switch
    {
        bool b => b,
        int i  => i != 0,
        string s => bool.TryParse(s, out var b) ? b : s != "0" && s.Length > 0,
        _ => Convert.ToBoolean(value),
    };
}
