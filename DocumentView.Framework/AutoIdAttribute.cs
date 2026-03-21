namespace DocumentView.Framework;

/// <summary>
/// Marks a static int field on a ResourceId class for auto-numbering.
/// Call <see cref="AutoIdAssigner.Assign"/> from the static constructor to assign sequential IDs.
/// When a control name is passed to the constructor, <see cref="ResourceIdResolver"/> uses it to find controls;
/// otherwise the field name is used as the control name.
/// </summary>
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed class AutoIdAttribute : Attribute
{
    public string? ControlName { get; }
    public AutoIdAttribute() { }
    public AutoIdAttribute(string controlName) => ControlName = controlName;
}
