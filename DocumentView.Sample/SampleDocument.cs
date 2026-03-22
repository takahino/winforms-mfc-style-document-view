using System.ComponentModel;
using System.Text;
using System.Text.Json;
using DocumentView.Framework;
using static DocumentView.Sample.ResourceId;

namespace DocumentView.Sample;

/// <summary>
/// One row of grid data.
/// Implements <see cref="INotifyPropertyChanged"/> so cell edits flow immediately to the document.
/// </summary>
public class ItemRow : INotifyPropertyChanged
{
    private int    _no;
    private string _name   = string.Empty;
    private bool   _active = true;

    public int No
    {
        get => _no;
        set { _no = value; OnPropertyChanged(nameof(No)); }
    }
    public string Name
    {
        get => _name;
        set { _name = value; OnPropertyChanged(nameof(Name)); }
    }
    public bool Active
    {
        get => _active;
        set { _active = value; OnPropertyChanged(nameof(Active)); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged(string name) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

/// <summary>
/// Sample employee document.
/// Add [DDX] + [DDV*] on m_* fields instead of hand-written DoDataExchange DDX_* / DDV_* lines.
/// </summary>
public partial class SampleDocument : MfcDocument
{
    public SampleDocument(IMessageBoxService messageBoxService) : base(messageBoxService) { }

    /// <summary>Directory for employee JSON files; tests may replace this.</summary>
    public static string DataDirectory { get; set; } =
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "employees");

    // ── DDX fields ─────────────────────────────────────────────────────────

    [DDX(SampleView.Ctrl.IDC_EDIT_NAME)]
    [DDVMaxChars(30)]
    public string m_strName = string.Empty;

    [DDX(SampleView.Ctrl.IDC_EDIT_AGE)]
    [DDVMinMax(0, 150)]
    public int m_nAge = 0;

    [DDX(SampleView.Ctrl.IDC_EDIT_MEMO)]
    [DDVMaxChars(200)]
    public string m_strMemo = string.Empty;

    // Active employee flag; when false, disables several input fields
    [DDX(SampleView.Ctrl.IDC_CHECK_ACTIVE)]
    public bool m_bActive = false;

    [DDX(SampleView.Ctrl.IDC_CHECK_NOTIFY)]
    public bool m_bNotify = false;

    [DDX(SampleView.Ctrl.IDC_COMBO_PREF)]
    public int m_nPrefIndex = 0;         // int → SelectedIndex

    [DDX(SampleView.Ctrl.IDC_COMBO_LANG)]
    public string m_strLang = string.Empty;  // string → Text

    [DDX(SampleView.Ctrl.IDC_SPIN_PRIORITY)]
    [DDVMinMax(1, 10)]
    public int m_nPriority = 1;

    [DDX(SampleView.Ctrl.IDC_RADIO_MALE)]
    public bool m_bMale = true;

    [DDX(SampleView.Ctrl.IDC_RADIO_FEMALE)]
    public bool m_bFemale = false;

    // Employment type
    [DDX(SampleView.Ctrl.IDC_RADIO_FULL_TIME)]
    public bool m_bFullTime = true;

    [DDX(SampleView.Ctrl.IDC_RADIO_CONTRACT)]
    public bool m_bContract = false;

    [DDX(SampleView.Ctrl.IDC_RADIO_PART_TIME)]
    public bool m_bPartTime = false;

    // Department list SelectedIndex
    [DDX(SampleView.Ctrl.IDC_LIST_ITEMS)]
    public int m_nItemIndex = -1;

    [DDX(SampleView.Ctrl.IDC_STATIC_STATUS)]
    public string m_strStatus = "Ready";

    // Project history; live-bound so edits apply immediately
    [DDX(SampleView.Ctrl.IDC_GRID_ITEMS)]
    public BindingList<ItemRow> m_gridItems = new()
    {
        new() { No = 1, Name = "Core system modernization", Active = true  },
        new() { No = 2, Name = "Mobile app development",    Active = true  },
        new() { No = 3, Name = "AI adoption support",         Active = false },
    };

    // Employee number (DDX last to keep existing ResourceId sequence stable)
    [DDX(SampleView.Ctrl.IDC_EDIT_EMPNO)]
    [DDVMaxChars(10)]
    public string m_strEmpNo = string.Empty;

    // ── Buttons / menu handlers ────────────────────────────────────────────

    /// <summary>Validates input and saves to a JSON file.</summary>
    public void OnBtnOk()
    {
        if (!UpdateData(true))   // UI → Document + DDV (message on failure)
            return;

        if (!ValidateBusinessRule(out string error))
        {
            MessageBoxService.Show(error, "Input error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            Directory.CreateDirectory(DataDirectory);
            var json = JsonSerializer.Serialize(ToData(), new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(Path.Combine(DataDirectory, $"{m_strEmpNo}.json"), json);
        }
        catch (Exception ex)
        {
            MessageBoxService.Show(
                $"Save failed.\n{ex.Message}", "Save error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        m_strStatus = $"Saved — [{m_strEmpNo}] {m_strName} (age {m_nAge})  {DateTime.Now:yyyy/MM/dd HH:mm}";
        UpdateData(false);
    }

    /// <summary>Loads JSON by employee number.</summary>
    public void OnMenuLoad()
    {
        // Read IDC_EDIT_EMPNO directly (avoid UpdateData(true) DDV)
        if (GetControl(IDC_EDIT_EMPNO) is TextBox tb)
            m_strEmpNo = tb.Text.Trim();

        if (string.IsNullOrWhiteSpace(m_strEmpNo))
        {
            MessageBoxService.Show(
                "Please enter an employee number.", "Load error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var path = Path.Combine(DataDirectory, $"{m_strEmpNo}.json");
        if (!File.Exists(path))
        {
            MessageBoxService.Show(
                $"No data found for employee number \"{m_strEmpNo}\".", "Load error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            var json = File.ReadAllText(path);
            var data = JsonSerializer.Deserialize<EmployeeData>(json)!;
            FromData(data);
        }
        catch (Exception ex)
        {
            MessageBoxService.Show(
                $"Load failed.\n{ex.Message}", "Load error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        m_strStatus = $"Loaded — [{m_strEmpNo}] {m_strName}";
        UpdateData(false);
    }

    /// <summary>Refreshes the UI from current document values.</summary>
    public void OnBtnCancel()
    {
        UpdateData(false);
    }

    /// <summary>Clears the form for new entry.</summary>
    public void OnNew()
    {
        m_strEmpNo   = string.Empty;
        m_strName    = string.Empty;
        m_nAge       = 0;
        m_strMemo    = string.Empty;
        m_bActive    = false;
        m_bNotify    = false;
        m_nPrefIndex = 0;
        m_strLang    = string.Empty;
        m_nPriority  = 1;
        m_bMale      = true;
        m_bFemale    = false;
        m_bFullTime  = true;
        m_bContract  = false;
        m_bPartTime  = false;
        m_nItemIndex = -1;
        m_gridItems  = new BindingList<ItemRow>();
        m_strStatus  = "Enter new employee data";
        UpdateData(false);
    }

    /// <summary>Debug: show DDX/DDV mapping list.</summary>
    public void OnBtnDebug()
    {
        var sb = new StringBuilder("=== DDX/DDV mapping ===\n");
        foreach (var (ctrl, member, type) in GetDdxMappings())
            sb.AppendLine($"  [{ctrl}] ↔ {member} ({type.Name})");
        MessageBoxService.Show(sb.ToString());
    }

    /// <summary>Debug: show grid data.</summary>
    public void OnBtnShowGrid()
    {
        var sb = new StringBuilder($"m_gridItems ({m_gridItems.Count} rows):\n\n");
        foreach (var row in m_gridItems)
            sb.AppendLine($"  No={row.No}  Name={row.Name}  Active={row.Active}");
        MessageBoxService.Show(sb.ToString(), "Grid data");
    }

    // ── ResourceId-based control helpers ───────────────────────────────────
    /// <summary>
    /// Enables or disables fields based on the Active checkbox.
    /// Called from the view CheckedChanged handler.
    /// </summary>
    public void OnCheckActiveChanged()
    {
        bool active = GetControl(IDC_CHECK_ACTIVE) is CheckBox cb && cb.Checked;

        SetEnabled(IDC_EDIT_NAME,     active);
        SetEnabled(IDC_EDIT_AGE,      active);
        SetEnabled(IDC_EDIT_MEMO,     active);
        SetEnabled(IDC_COMBO_PREF,    active);
        SetEnabled(IDC_COMBO_LANG,    active);
        SetEnabled(IDC_SPIN_PRIORITY, active);
    }

    // ── Business rules ─────────────────────────────────────────────────────
    public bool ValidateBusinessRule(out string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(m_strEmpNo))
        {
            errorMessage = "Employee number is required.";
            return false;
        }
        if (string.IsNullOrWhiteSpace(m_strName))
        {
            errorMessage = "Name is required.";
            return false;
        }
        errorMessage = string.Empty;
        return true;
    }

    // ── Serialization ──────────────────────────────────────────────────────
    private EmployeeData ToData() => new()
    {
        EmpNo     = m_strEmpNo,
        Name      = m_strName,
        Age       = m_nAge,
        Memo      = m_strMemo,
        Active    = m_bActive,
        Notify    = m_bNotify,
        PrefIndex = m_nPrefIndex,
        Lang      = m_strLang,
        Priority  = m_nPriority,
        Male      = m_bMale,
        Female    = m_bFemale,
        FullTime  = m_bFullTime,
        Contract  = m_bContract,
        PartTime  = m_bPartTime,
        ItemIndex = m_nItemIndex,
        Projects  = m_gridItems
            .Select(r => new ProjectItemData { No = r.No, Name = r.Name, Active = r.Active })
            .ToList(),
    };

    private void FromData(EmployeeData d)
    {
        m_strEmpNo   = d.EmpNo;
        m_strName    = d.Name;
        m_nAge       = d.Age;
        m_strMemo    = d.Memo;
        m_bActive    = d.Active;
        m_bNotify    = d.Notify;
        m_nPrefIndex = d.PrefIndex;
        m_strLang    = d.Lang;
        m_nPriority  = d.Priority;
        m_bMale      = d.Male;
        m_bFemale    = d.Female;
        m_bFullTime  = d.FullTime;
        m_bContract  = d.Contract;
        m_bPartTime  = d.PartTime;
        m_nItemIndex = d.ItemIndex;
        m_gridItems  = new BindingList<ItemRow>(
            d.Projects.Select(p => new ItemRow { No = p.No, Name = p.Name, Active = p.Active }).ToList());
    }
}

/// <summary>DTO for JSON serialization.</summary>
public sealed class EmployeeData
{
    public string EmpNo     { get; set; } = string.Empty;
    public string Name      { get; set; } = string.Empty;
    public int    Age       { get; set; }
    public string Memo      { get; set; } = string.Empty;
    public bool   Active    { get; set; }
    public bool   Notify    { get; set; }
    public int    PrefIndex { get; set; }
    public string Lang      { get; set; } = string.Empty;
    public int    Priority  { get; set; } = 1;
    public bool   Male      { get; set; } = true;
    public bool   Female    { get; set; }
    public bool   FullTime  { get; set; } = true;
    public bool   Contract  { get; set; }
    public bool   PartTime  { get; set; }
    public int    ItemIndex { get; set; } = -1;
    public List<ProjectItemData> Projects { get; set; } = [];
}

public sealed class ProjectItemData
{
    public int    No     { get; set; }
    public string Name   { get; set; } = string.Empty;
    public bool   Active { get; set; }
}
