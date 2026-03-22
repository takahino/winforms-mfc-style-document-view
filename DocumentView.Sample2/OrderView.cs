using System.Text.Json;
using DocumentView.Framework;

namespace DocumentView.Sample2;

public partial class OrderView : Form
{
    /// <summary>
    /// Control name accessors for DDX binding.
    /// Exposes private designer field names via <c>nameof</c>.
    /// </summary>
    public static class Ctrl
    {
        public const string IDC_EDIT_SUPPLIER_CODE  = nameof(OrderView.IDC_EDIT_SUPPLIER_CODE);
        public const string IDC_EDIT_SUPPLIER_NAME  = nameof(OrderView.IDC_EDIT_SUPPLIER_NAME);
        public const string IDC_EDIT_ADDRESS        = nameof(OrderView.IDC_EDIT_ADDRESS);
        public const string IDC_EDIT_TEL            = nameof(OrderView.IDC_EDIT_TEL);
        public const string IDC_EDIT_FAX            = nameof(OrderView.IDC_EDIT_FAX);
        public const string IDC_EDIT_ORDER_NO       = nameof(OrderView.IDC_EDIT_ORDER_NO);
        public const string IDC_EDIT_ORDER_DATE     = nameof(OrderView.IDC_EDIT_ORDER_DATE);
        public const string IDC_EDIT_DELIVERY_DATE  = nameof(OrderView.IDC_EDIT_DELIVERY_DATE);
        public const string IDC_COMBO_STATUS        = nameof(OrderView.IDC_COMBO_STATUS);
        public const string IDC_CHECK_URGENT        = nameof(OrderView.IDC_CHECK_URGENT);
        public const string IDC_EDIT_MEMO           = nameof(OrderView.IDC_EDIT_MEMO);
        public const string IDC_GRID_LINES          = nameof(OrderView.IDC_GRID_LINES);
        public const string IDC_LABEL_TOTAL         = nameof(OrderView.IDC_LABEL_TOTAL);
    }

    // ── 3つの独立Document（MFC の 3ダイアログに対応）──────────────────────
    public SupplierDocument     SupplierDoc { get; }
    public OrderHeaderDocument  HeaderDoc   { get; }
    public OrderDetailDocument  DetailDoc   { get; }

    private readonly IMessageBoxService _messageBoxService;

    /// <summary>発注データの保存ディレクトリ。テストで差し替え可能。</summary>
    public static string DataDirectory { get; set; } =
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "orders");

    /// <summary>WinForms デザイナー用。実行時は DI コンテナが使用するコンストラクタを使う。</summary>
    public OrderView() : this(null!, null!, null!, null!) { }

    /// <summary>DI コンテナが使用するコンストラクタ。3つのDocumentを受け取る。</summary>
    public OrderView(
        SupplierDocument    supplierDoc,
        OrderHeaderDocument headerDoc,
        OrderDetailDocument detailDoc,
        IMessageBoxService  messageBoxService)
    {
        SupplierDoc        = supplierDoc;
        HeaderDoc          = headerDoc;
        DetailDoc          = detailDoc;
        _messageBoxService = messageBoxService;

        InitializeComponent();
        if (supplierDoc is null) return;

        IDC_COMBO_STATUS.Items.AddRange(
            ["Not ordered", "Ordered", "Awaiting delivery", "Received"]);

        // 3つのDocumentを同一Formにアタッチ（FindControl は searchAllChildren=true）
        SupplierDoc.AttachView(this);
        HeaderDoc  .AttachView(this);
        DetailDoc  .AttachView(this);

        // 全Documentが同一ResourceIdを共有（13個のIDC_*をすべてマップ）
        SupplierDoc.AttachResourceId(typeof(ResourceId));
        HeaderDoc  .AttachResourceId(typeof(ResourceId));
        DetailDoc  .AttachResourceId(typeof(ResourceId));

        IDC_COMBO_STATUS.SelectedIndexChanged += (_, _) => HeaderDoc.OnStatusChanged();

        // Debug panel: subscribe to framework events from all 3 documents
        SupplierDoc.DebugLog    += msg => AppendLog("[Supplier] " + msg);
        HeaderDoc  .DebugLog    += msg => AppendLog("[Header] " + msg);
        DetailDoc  .DebugLog    += msg => AppendLog("[Lines] " + msg);

        SupplierDoc.DataUpdated += _ => RefreshDdxState();
        HeaderDoc  .DataUpdated += _ => RefreshDdxState();
        DetailDoc  .DataUpdated += _ => RefreshDdxState();
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        // SplitContainer: SplitterDistance は Panel1MinSize ～ (合計 − Panel2MinSize − バー) に収める
        SetSplitterDistanceFraction(splitMain,  0.55);
        SetSplitterDistanceFraction(splitLeft1, 0.38);
        SetSplitterDistanceFraction(splitLeft2, 0.52);
        SetSplitterDistanceFraction(splitDebug, 0.40);
        SetupDebugGrid();
        OnNew();
        SetGridColumnHeaders();
    }

    /// <summary>
    /// <see cref="SplitContainer.SplitterDistance"/> を、MinSize 制約内に収めて設定する。
    /// </summary>
    private static void SetSplitterDistanceFraction(SplitContainer split, double fraction)
    {
        int total = split.Orientation == Orientation.Vertical ? split.Width : split.Height;
        if (total <= split.SplitterWidth + 1) return;

        int max = total - split.Panel2MinSize - split.SplitterWidth;
        int min = split.Panel1MinSize;
        if (max < min) return;

        int desired = (int)Math.Round(total * fraction);
        split.SplitterDistance = Math.Clamp(desired, min, max);
    }

    // ── Commands ────────────────────────────────────────────────────────────

    /// <summary>全Documentをリセットして新規発注を開始する。</summary>
    public void OnNew()
    {
        SupplierDoc.Reset();
        HeaderDoc  .Reset(GenerateOrderNo());
        DetailDoc  .Reset();
        DetailDoc  .SubscribeGridLines();

        SupplierDoc.UpdateData(false);
        HeaderDoc  .UpdateData(false);
        DetailDoc  .UpdateData(false);
    }

    /// <summary>全DocumentのUI→Doc転送・DDV検証・JSON保存を行う。</summary>
    public void OnBtnSave()
    {
        if (!SupplierDoc.UpdateData(true)) return;
        if (!HeaderDoc  .UpdateData(true)) return;
        if (!DetailDoc  .UpdateData(true)) return;

        if (!SupplierDoc.ValidateBusinessRule(out var error))
        {
            _messageBoxService.Show(error, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            Directory.CreateDirectory(DataDirectory);
            var json = JsonSerializer.Serialize(
                ToData(), new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(
                Path.Combine(DataDirectory, $"{HeaderDoc.m_strOrderNo}.json"), json);
        }
        catch (Exception ex)
        {
            _messageBoxService.Show(
                $"Save failed.\n{ex.Message}", "Save error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    /// <summary>HeaderDoc.m_strOrderNo に対応するJSONを読み込む。</summary>
    public void OnMenuLoad()
    {
        if (string.IsNullOrWhiteSpace(HeaderDoc.m_strOrderNo))
        {
            _messageBoxService.Show(
                "Enter an order number.", "Load error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var path = Path.Combine(DataDirectory, $"{HeaderDoc.m_strOrderNo}.json");
        if (!File.Exists(path))
        {
            _messageBoxService.Show(
                $"No data found for order \"{HeaderDoc.m_strOrderNo}\".", "Load error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            var json = File.ReadAllText(path);
            var data = JsonSerializer.Deserialize<PurchaseOrderData>(json)!;
            FromData(data);
        }
        catch (Exception ex)
        {
            _messageBoxService.Show(
                $"Load failed.\n{ex.Message}", "Load error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        DetailDoc.SubscribeGridLines();
        SupplierDoc.UpdateData(false);
        HeaderDoc  .UpdateData(false);
        DetailDoc  .UpdateData(false);
    }

    // ── Serialization ────────────────────────────────────────────────────────

    private PurchaseOrderData ToData() => new()
    {
        SupplierCode = SupplierDoc.m_strSupplierCode,
        SupplierName = SupplierDoc.m_strSupplierName,
        Address      = SupplierDoc.m_strAddress,
        Tel          = SupplierDoc.m_strTel,
        Fax          = SupplierDoc.m_strFax,
        OrderNo      = HeaderDoc.m_strOrderNo,
        OrderDate    = HeaderDoc.m_strOrderDate,
        DeliveryDate = HeaderDoc.m_strDeliveryDate,
        Status       = HeaderDoc.m_nStatus,
        Urgent       = HeaderDoc.m_bUrgent,
        Memo         = HeaderDoc.m_strMemo,
        Lines        = DetailDoc.m_gridLines
            .Select(l => new OrderLineData
            {
                ItemCode  = l.ItemCode,
                ItemName  = l.ItemName,
                Qty       = l.Qty,
                UnitPrice = l.UnitPrice,
            })
            .ToList(),
    };

    private void FromData(PurchaseOrderData d)
    {
        SupplierDoc.m_strSupplierCode = d.SupplierCode;
        SupplierDoc.m_strSupplierName = d.SupplierName;
        SupplierDoc.m_strAddress      = d.Address;
        SupplierDoc.m_strTel          = d.Tel;
        SupplierDoc.m_strFax          = d.Fax;

        HeaderDoc.m_strOrderNo      = d.OrderNo;
        HeaderDoc.m_strOrderDate    = d.OrderDate;
        HeaderDoc.m_strDeliveryDate = d.DeliveryDate;
        HeaderDoc.m_nStatus         = d.Status;
        HeaderDoc.m_bUrgent         = d.Urgent;
        HeaderDoc.m_strMemo         = d.Memo;

        DetailDoc.m_gridLines = new System.ComponentModel.BindingList<OrderLine>(
            d.Lines.Select(l => new OrderLine
            {
                ItemCode  = l.ItemCode,
                ItemName  = l.ItemName,
                Qty       = l.Qty,
                UnitPrice = l.UnitPrice,
            }).ToList());
    }

    // ── Order number generation ──────────────────────────────────────────────

    private string GenerateOrderNo()
    {
        var dateStr = DateTime.Today.ToString("yyyyMMdd");
        var prefix  = $"PO-{dateStr}_";
        int maxSeq  = 0;

        if (Directory.Exists(DataDirectory))
        {
            foreach (var file in Directory.GetFiles(DataDirectory, $"{prefix}*.json"))
            {
                var stem = Path.GetFileNameWithoutExtension(file);
                if (stem.StartsWith(prefix) &&
                    int.TryParse(stem.AsSpan(prefix.Length), out int seq))
                    maxSeq = Math.Max(maxSeq, seq);
            }
        }

        return $"{prefix}{maxSeq + 1:D4}";
    }

    // ── Button / Menu handlers ───────────────────────────────────────────────

    private void btnNew_Click(object sender, EventArgs e)    => OnNew();
    private void btnSave_Click(object sender, EventArgs e)   => OnBtnSave();
    private void btnCancel_Click(object sender, EventArgs e)
    {
        SupplierDoc.UpdateData(false);
        HeaderDoc  .UpdateData(false);
        DetailDoc  .UpdateData(false);
    }

    private void menuFileNew_Click(object sender, EventArgs e)  => OnNew();
    private void menuFileSave_Click(object sender, EventArgs e) => OnBtnSave();
    private void menuFileExit_Click(object sender, EventArgs e) => Close();

    // ── Debug panel ─────────────────────────────────────────────────────────

    private void btnClearLog_Click(object sender, EventArgs e)       => rtbOpLog.Clear();
    private void btnRefreshState_Click(object sender, EventArgs e)   => RefreshDdxState();
    private void btnRunUpdateFalse_Click(object sender, EventArgs e)
    {
        SupplierDoc.UpdateData(false);
        HeaderDoc  .UpdateData(false);
        DetailDoc  .UpdateData(false);
    }
    private void btnRunUpdateTrue_Click(object sender, EventArgs e)
    {
        SupplierDoc.UpdateData(true);
        HeaderDoc  .UpdateData(true);
        DetailDoc  .UpdateData(true);
    }

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

    private void RefreshDdxState()
    {
        if (dgvDdxState.IsDisposed || dgvDdxState.Columns.Count == 0) return;

        dgvDdxState.SuspendLayout();
        dgvDdxState.Rows.Clear();

        // 3つのDocumentのDDX状態を結合して表示
        var allRows = SupplierDoc.GetDdxState()
            .Concat(HeaderDoc.GetDdxState())
            .Concat(DetailDoc.GetDdxState());

        foreach (var (ctrl, field, ctrlVal, docVal, inSync) in allRows)
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

    private void AppendLog(string message)
    {
        if (rtbOpLog.IsDisposed) return;

        System.Drawing.Color color;
        bool bold;

        if (message.Contains("UpdateData"))
            (color, bold) = (System.Drawing.Color.CornflowerBlue, true);
        else if (message.Contains("→ OK"))
            (color, bold) = (System.Drawing.Color.LightGreen, false);
        else if (message.Contains("→ NG"))
            (color, bold) = (System.Drawing.Color.Tomato, true);
        else if (message.Contains("Control not found"))
            (color, bold) = (System.Drawing.Color.Orange, false);
        else if (message.Contains("DDV"))
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

    private void menuFileOpen_Click(object sender, EventArgs e)
    {
        if (!Directory.Exists(DataDirectory))
            Directory.CreateDirectory(DataDirectory);

        using var dlg = new OpenFileDialog
        {
            Title            = "Open order data",
            Filter           = "Order data (*.json)|*.json",
            InitialDirectory = DataDirectory,
        };

        if (dlg.ShowDialog() != DialogResult.OK) return;

        HeaderDoc.m_strOrderNo = Path.GetFileNameWithoutExtension(dlg.FileName);
        OnMenuLoad();
    }

    // ── Grid column setup ────────────────────────────────────────────────────

    private void SetGridColumnHeaders()
    {
        if (IDC_GRID_LINES.Columns.Count > 0)
        {
            if (IDC_GRID_LINES.Columns[nameof(OrderLine.ItemCode)]  is { } c1) c1.HeaderText = "Part no.";
            if (IDC_GRID_LINES.Columns[nameof(OrderLine.ItemName)]  is { } c2) c2.HeaderText = "Part name";
            if (IDC_GRID_LINES.Columns[nameof(OrderLine.Qty)]       is { } c3) c3.HeaderText = "Qty";
            if (IDC_GRID_LINES.Columns[nameof(OrderLine.UnitPrice)] is { } c4) c4.HeaderText = "Unit price";
            if (IDC_GRID_LINES.Columns[nameof(OrderLine.Amount)]    is { } c5)
            {
                c5.HeaderText = "Amount";
                c5.ReadOnly   = true;
            }
        }
    }
}
