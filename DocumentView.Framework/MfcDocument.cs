using System.Reflection;

namespace DocumentView.Framework;

public abstract class MfcDocument
{
    protected record DdxEntry(
        string                      MemberName,
        Type                        MemberType,
        string                      ControlName,
        string?                     ControlProperty,
        Func<object?>               Getter,
        Action<object>              Setter,
        IReadOnlyList<DDVAttribute> Validators
    );

    private List<DdxEntry>? _ddxEntries;
    private Control? _view;
    private Dictionary<int, Control>? _resourceMap;

    /// <summary>Message box service injected from the DI container or a test mock.</summary>
    protected IMessageBoxService MessageBoxService { get; }

    // ── Debug events ────────────────────────────────────────────────────────

    /// <summary>Fires for each DDX/DDV step. No overhead when no handler is attached.</summary>
    public event Action<string>? DebugLog;

    /// <summary>Fires after the DDX phase completes (before DDV). When saveAndValidate is true, UI→Doc transfer is done; otherwise Doc→UI.</summary>
    public event Action<bool>? DataUpdated;

    private void Log(string msg) => DebugLog?.Invoke(msg);

    protected MfcDocument(IMessageBoxService messageBoxService)
    {
        MessageBoxService = messageBoxService;
    }

    /// <summary>Public API equivalent to MFC InitializeDdx. Call from the view constructor.</summary>
    public void AttachView(Form view)
    {
        _view = view;
        _ddxEntries = BuildEntries();
    }

    /// <summary>
    /// Equivalent to MFC UpdateData(BOOL).
    /// true  = UI → Document + DDV validation (UpdateData(TRUE))
    /// false = Document → UI (UpdateData(FALSE))
    /// On DDV failure, shows a message box, moves focus, and returns false.
    /// </summary>
    public bool UpdateData(bool saveAndValidate)
    {
        if (_view is null || _ddxEntries is null)
            return false;

        int count = _ddxEntries.Count;
        Log(saveAndValidate
            ? $"UpdateData(true) — UI→Doc: {count} field(s)"
            : $"UpdateData(false) — Doc→UI: {count} field(s)");

        // ── DDX phase ───────────────────────────────────────────────────────
        foreach (var entry in _ddxEntries)
        {
            var ctrl = FindControl(_view, entry.ControlName);
            if (ctrl is null)
            {
                Log($"  [{entry.ControlName}] Control not found");
                continue;
            }

            if (saveAndValidate)
            {
                var newVal = ControlValueConverter.GetValue(ctrl, entry.MemberType, entry.ControlProperty);
                if (newVal is not null)
                {
                    entry.Setter(newVal);
                    Log($"  [{entry.ControlName}] → {entry.MemberName} = {FormatValue(newVal)}");
                }
            }
            else
            {
                var val = entry.Getter();
                ControlValueConverter.SetValue(ctrl, val, entry.ControlProperty);
                Log($"  [{entry.ControlName}] ← {entry.MemberName} = {FormatValue(val)}");
            }
        }

        // Notify after DDX completes (before DDV)
        DataUpdated?.Invoke(saveAndValidate);

        // ── DDV phase (saveAndValidate only) ───────────────────────────────
        if (saveAndValidate)
        {
            foreach (var entry in _ddxEntries)
            {
                foreach (var ddv in entry.Validators)
                {
                    var val = entry.Getter();
                    string ddvName = ddv.GetType().Name.Replace("Attribute", "");
                    if (!ddv.Validate(val, out string msg))
                    {
                        Log($"  DDV [{entry.ControlName}] {ddvName} → NG ({msg})");
                        var ctrl = FindControl(_view, entry.ControlName);
                        if (ctrl is not null)
                        {
                            ctrl.Focus();
                            if (ctrl is TextBox tb) tb.SelectAll();
                        }
                        MessageBoxService.Show(msg, "Validation error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                    Log($"  DDV [{entry.ControlName}] {ddvName} → OK");
                }
            }
            Log("  DDV: all passed");
        }

        return true;
    }

    /// <summary>
    /// Registers a ResourceId type and builds an int (IDC_* value) → Control map.
    /// Call after AttachView so business logic can use SetEnabled / SetVisible / GetControl.
    /// </summary>
    public void AttachResourceId(Type resourceIdType)
    {
        if (_view is null) return;
        var resolver = new ResourceIdResolver(resourceIdType);
        _resourceMap = [];
        foreach (var (id, name) in resolver.AllEntries)
        {
            var ctrl = FindControl(_view, name);
            if (ctrl is not null)
                _resourceMap[id] = ctrl;
        }
    }

    /// <summary>Gets a control by ResourceId int value.</summary>
    protected Control? GetControl(int resourceId)
        => _resourceMap?.GetValueOrDefault(resourceId);

    /// <summary>Sets Enabled by ResourceId int value.</summary>
    protected void SetEnabled(int resourceId, bool enabled)
    {
        if (GetControl(resourceId) is { } ctrl)
            ctrl.Enabled = enabled;
    }

    /// <summary>Sets Visible by ResourceId int value.</summary>
    protected void SetVisible(int resourceId, bool visible)
    {
        if (GetControl(resourceId) is { } ctrl)
            ctrl.Visible = visible;
    }

    /// <summary>
    /// Builds the DDX entry list. Override in generated partial classes to replace
    /// reflection with compile-time direct field access for better performance.
    /// </summary>
    protected virtual List<DdxEntry> BuildEntries()
    {
        var list = new List<DdxEntry>();
        const BindingFlags flags =
            BindingFlags.Public | BindingFlags.NonPublic |
            BindingFlags.Instance | BindingFlags.FlattenHierarchy;

        foreach (var fi in GetType().GetFields(flags))
        {
            var captured = fi;
            foreach (var attr in fi.GetCustomAttributes<DDXAttribute>(true))
                list.Add(new DdxEntry(
                    fi.Name, fi.FieldType, attr.ControlName, attr.ControlProperty,
                    Getter:     () => captured.GetValue(this),
                    Setter:     v  => captured.SetValue(this, v),
                    Validators: fi.GetCustomAttributes<DDVAttribute>(true).ToArray()
                ));
        }

        foreach (var pi in GetType().GetProperties(flags))
        {
            var captured = pi;
            foreach (var attr in pi.GetCustomAttributes<DDXAttribute>(true))
                list.Add(new DdxEntry(
                    pi.Name, pi.PropertyType, attr.ControlName, attr.ControlProperty,
                    Getter:     () => captured.GetValue(this),
                    Setter:     v  => { if (captured.CanWrite) captured.SetValue(this, v); },
                    Validators: pi.GetCustomAttributes<DDVAttribute>(true).ToArray()
                ));
        }

        return list;
    }

    private static Control? FindControl(Control parent, string name)
    {
        var found = parent.Controls.Find(name, searchAllChildren: true);
        return found.Length > 0 ? found[0] : null;
    }

    /// <summary>Debug: returns the list of DDX mappings.</summary>
    public IReadOnlyList<(string ControlName, string MemberName, Type MemberType)> GetDdxMappings()
    {
        if (_ddxEntries is null) return [];
        return _ddxEntries
            .Select(e => (e.ControlName, e.MemberName, e.MemberType))
            .ToList();
    }

    /// <summary>
    /// Debug: current values per DDX field (control value / document value / in sync).
    /// </summary>
    public IReadOnlyList<(string ControlName, string MemberName, string ControlValue, string DocumentValue, bool InSync)>
        GetDdxState()
    {
        if (_view is null || _ddxEntries is null) return [];

        var result = new List<(string, string, string, string, bool)>(_ddxEntries.Count);
        foreach (var e in _ddxEntries)
        {
            var ctrl    = FindControl(_view, e.ControlName);
            var ctrlVal = ctrl is null ? "—" : ControlDisplayValue(ctrl);
            var docVal  = FormatValue(e.Getter());
            result.Add((e.ControlName, e.MemberName, ctrlVal, docVal, ctrlVal == docVal));
        }
        return result;
    }

    // ── Display helpers ─────────────────────────────────────────────────────

    private static string ControlDisplayValue(Control ctrl) => ctrl switch
    {
        TextBox tb                                                   => tb.Text,
        CheckBox cb                                                  => cb.Checked ? "true" : "false",
        RadioButton rb                                               => rb.Checked ? "true" : "false",
        ComboBox { DropDownStyle: ComboBoxStyle.DropDownList } cmb   => cmb.SelectedIndex.ToString(),
        ComboBox cmb                                                 => cmb.Text,
        NumericUpDown nud                                            => ((int)nud.Value).ToString(),
        ListBox lb                                                   => lb.SelectedIndex.ToString(),
        Label lbl                                                    => lbl.Text,
        DataGridView dgv when dgv.DataSource is System.Collections.ICollection col
                                                                     => $"({col.Count} items)",
        DataGridView dgv => $"({Math.Max(0, dgv.RowCount - (dgv.AllowUserToAddRows ? 1 : 0))} items)",
        _                                                            => ctrl.GetType().Name,
    };

    private static string FormatValue(object? val) => val switch
    {
        null                               => "(null)",
        bool b                             => b ? "true" : "false",
        System.Collections.ICollection col => $"({col.Count} items)",
        _                                  => val.ToString() ?? "(null)",
    };
}
