namespace DocumentView.Framework;

/// <summary>
/// Equivalent to MFC DoDataExchange DDX_* macros.
/// Apply on Document m_* fields to bind the control name.
///
/// MFC:  DDX_Text(pDX, IDC_EDIT_NAME, m_strName);
/// C#:   [DDX("IDC_EDIT_NAME")]
///       public string m_strName = "";
/// </summary>
[AttributeUsage(
    AttributeTargets.Field | AttributeTargets.Property,
    AllowMultiple = true,
    Inherited = true)]
public sealed class DDXAttribute : Attribute
{
    /// <summary>WinForms control Name property.</summary>
    public string ControlName { get; }

    /// <summary>
    /// e.g. "SelectedIndex", "Text", "SelectedItem".
    /// When null, <see cref="ControlValueConverter"/> infers from field type.
    /// Binding ComboBox to int selects SelectedIndex automatically.
    /// </summary>
    public string? ControlProperty { get; init; }

    public DDXAttribute(string controlName) => ControlName = controlName;
}
