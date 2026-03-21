namespace DocumentView.Sample;

public partial class SampleView : Form
{
    /// <summary>
    /// Control name accessors for DDX binding.
    /// Exposes private designer field names via <c>nameof</c>.
    /// </summary>
    public static class Ctrl
    {
        public const string IDC_EDIT_EMPNO    = nameof(SampleView.IDC_EDIT_EMPNO);
        public const string IDC_EDIT_NAME     = nameof(SampleView.IDC_EDIT_NAME);
        public const string IDC_EDIT_AGE      = nameof(SampleView.IDC_EDIT_AGE);
        public const string IDC_EDIT_MEMO     = nameof(SampleView.IDC_EDIT_MEMO);
        public const string IDC_CHECK_ACTIVE  = nameof(SampleView.IDC_CHECK_ACTIVE);
        public const string IDC_CHECK_NOTIFY  = nameof(SampleView.IDC_CHECK_NOTIFY);
        public const string IDC_COMBO_PREF    = nameof(SampleView.IDC_COMBO_PREF);
        public const string IDC_COMBO_LANG    = nameof(SampleView.IDC_COMBO_LANG);
        public const string IDC_SPIN_PRIORITY = nameof(SampleView.IDC_SPIN_PRIORITY);
        public const string IDC_RADIO_MALE       = nameof(SampleView.IDC_RADIO_MALE);
        public const string IDC_RADIO_FEMALE     = nameof(SampleView.IDC_RADIO_FEMALE);
        public const string IDC_RADIO_FULL_TIME  = nameof(SampleView.IDC_RADIO_FULL_TIME);
        public const string IDC_RADIO_CONTRACT   = nameof(SampleView.IDC_RADIO_CONTRACT);
        public const string IDC_RADIO_PART_TIME  = nameof(SampleView.IDC_RADIO_PART_TIME);
        public const string IDC_LIST_ITEMS    = nameof(SampleView.IDC_LIST_ITEMS);
        public const string IDC_STATIC_STATUS = nameof(SampleView.IDC_STATIC_STATUS);
        public const string IDC_GRID_ITEMS    = nameof(SampleView.IDC_GRID_ITEMS);
    }

    public SampleDocument Document { get; }

    /// <summary>For the WinForms designer. At runtime the DI container uses <see cref="SampleView(SampleDocument)"/>.</summary>
    public SampleView() : this(null!) { }

    /// <summary>Constructor used by the DI container.</summary>
    public SampleView(SampleDocument document)
    {
        Document = document;
        InitializeComponent();
        if (document is null) return;

        IDC_COMBO_PREF.Items.AddRange([
            "Tokyo", "Kanagawa", "Osaka", "Aichi", "Saitama",
            "Chiba", "Hyogo", "Hokkaido", "Fukuoka", "Miyagi"
        ]);
        IDC_COMBO_LANG.Items.AddRange(["C#", "C++", "Java", "Python", "TypeScript", "Go", "Rust", "Kotlin"]);
        IDC_LIST_ITEMS.Items.AddRange(["Development", "Infrastructure", "Sales", "HR", "Accounting", "Legal"]);

        Document.AttachView(this);
        Document.AttachResourceId(typeof(ResourceId));
        IDC_CHECK_ACTIVE.CheckedChanged += (_, _) => Document.OnCheckActiveChanged();

        // Debug panel: subscribe to framework events
        Document.DebugLog   += msg  => AppendLog(msg);
        Document.DataUpdated += _   => RefreshDdxState();
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        SetupDebugGrid();
        Document.UpdateData(false);
        SetGridColumnHeaders();
        Document.OnCheckActiveChanged();
    }

    // ── Commands / menu ─────────────────────────────────────────────────────
    private void btnOk_Click(object sender, EventArgs e)     => Document.OnBtnOk();
    private void btnCancel_Click(object sender, EventArgs e) => Document.OnBtnCancel();
    private void btnNew_Click(object sender, EventArgs e)    => Document.OnNew();

    private void menuFileNew_Click(object sender, EventArgs e)  => Document.OnNew();
    private void menuFileLoad_Click(object sender, EventArgs e) => Document.OnMenuLoad();
    private void menuFileSave_Click(object sender, EventArgs e) => Document.OnBtnOk();
    private void menuFileExit_Click(object sender, EventArgs e) => this.Close();

    // ── Debug panel ─────────────────────────────────────────────────────────
    private void btnClearLog_Click(object sender, EventArgs e)       => rtbOpLog.Clear();
    private void btnRefreshState_Click(object sender, EventArgs e)   => RefreshDdxState();
    private void btnRunUpdateFalse_Click(object sender, EventArgs e) => Document.UpdateData(false);
    private void btnRunUpdateTrue_Click(object sender, EventArgs e)  => Document.UpdateData(true);

    /// <summary>Initializes columns for the DDX state grid.</summary>
    private void SetupDebugGrid()
    {
        dgvDdxState.Columns.AddRange(
            new DataGridViewTextBoxColumn { Name = "colCtrl",    HeaderText = "Control",   FillWeight = 130 },
            new DataGridViewTextBoxColumn { Name = "colField",   HeaderText = "Field",     FillWeight = 110 },
            new DataGridViewTextBoxColumn { Name = "colCtrlVal", HeaderText = "UI value",  FillWeight =  85 },
            new DataGridViewTextBoxColumn { Name = "colDocVal",  HeaderText = "Doc value", FillWeight =  85 },
            new DataGridViewTextBoxColumn { Name = "colSync",    HeaderText = "Match",     FillWeight =  40 }
        );
        dgvDdxState.RowTemplate.Height = 22;
    }

    /// <summary>Refreshes the DDX state grid from current Document ↔ UI values.</summary>
    private void RefreshDdxState()
    {
        if (dgvDdxState.IsDisposed) return;

        dgvDdxState.SuspendLayout();
        dgvDdxState.Rows.Clear();

        foreach (var (ctrl, field, ctrlVal, docVal, inSync) in Document.GetDdxState())
        {
            int i = dgvDdxState.Rows.Add(ctrl, field, ctrlVal, docVal, inSync ? "✓" : "✗");

            var row = dgvDdxState.Rows[i];
            row.DefaultCellStyle.BackColor = inSync
                ? System.Drawing.Color.FromArgb(235, 255, 235)
                : System.Drawing.Color.FromArgb(255, 235, 235);
            row.Cells["colSync"].Style.ForeColor = inSync
                ? System.Drawing.Color.DarkGreen
                : System.Drawing.Color.Crimson;
            row.Cells["colSync"].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        dgvDdxState.ResumeLayout();
    }

    /// <summary>Appends one colored line to the operation log.</summary>
    private void AppendLog(string message)
    {
        if (rtbOpLog.IsDisposed) return;

        System.Drawing.Color color;
        bool bold;

        if (message.StartsWith("UpdateData"))
            (color, bold) = (System.Drawing.Color.CornflowerBlue, true);
        else if (message.Contains("→ OK"))
            (color, bold) = (System.Drawing.Color.LightGreen, false);
        else if (message.Contains("→ NG"))
            (color, bold) = (System.Drawing.Color.Tomato, true);
        else if (message.Contains("Control not found"))
            (color, bold) = (System.Drawing.Color.Orange, false);
        else if (message.TrimStart().StartsWith("DDV"))
            (color, bold) = (System.Drawing.Color.Cyan, false);
        else if (message.TrimStart().StartsWith("["))
            (color, bold) = (System.Drawing.Color.Silver, false);
        else
            (color, bold) = (System.Drawing.Color.LightGray, false);

        rtbOpLog.SelectionStart  = rtbOpLog.TextLength;
        rtbOpLog.SelectionLength = 0;
        rtbOpLog.SelectionColor  = color;
        rtbOpLog.SelectionFont   = bold
            ? new System.Drawing.Font(rtbOpLog.Font, System.Drawing.FontStyle.Bold)
            : rtbOpLog.Font;
        rtbOpLog.AppendText($"[{DateTime.Now:HH:mm:ss.fff}] {message}\n");
        rtbOpLog.ScrollToCaret();
    }

    /// <summary>Sets DataGridView column headers (call after UpdateData(false)).</summary>
    private void SetGridColumnHeaders()
    {
        if (IDC_GRID_ITEMS.Columns.Count > 0)
        {
            if (IDC_GRID_ITEMS.Columns[nameof(ItemRow.No)]     is { } c1) c1.HeaderText = "No.";
            if (IDC_GRID_ITEMS.Columns[nameof(ItemRow.Name)]   is { } c2) c2.HeaderText = "Project";
            if (IDC_GRID_ITEMS.Columns[nameof(ItemRow.Active)] is { } c3) c3.HeaderText = "Active";
        }
    }
}
