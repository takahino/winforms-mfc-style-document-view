namespace DocumentView.Sample2;

partial class OrderView
{
    private System.ComponentModel.IContainer components = null!;

    // ── Menu ───────────────────────────────────────────────────────────────
    private System.Windows.Forms.MenuStrip          menuStrip1   = null!;
    private System.Windows.Forms.ToolStripMenuItem  menuFile     = null!;
    private System.Windows.Forms.ToolStripMenuItem  menuFileNew  = null!;
    private System.Windows.Forms.ToolStripMenuItem  menuFileOpen = null!;
    private System.Windows.Forms.ToolStripSeparator menuFileSep1 = null!;
    private System.Windows.Forms.ToolStripMenuItem  menuFileSave = null!;
    private System.Windows.Forms.ToolStripSeparator menuFileSep2 = null!;
    private System.Windows.Forms.ToolStripMenuItem  menuFileExit = null!;

    // ── Splitters ──────────────────────────────────────────────────────────
    private System.Windows.Forms.SplitContainer splitMain   = null!; // 左(フォーム) | 右(デバッグ)
    private System.Windows.Forms.SplitContainer splitLeft1  = null!; // 仕入先 | (ヘッダ+明細)
    private System.Windows.Forms.SplitContainer splitLeft2  = null!; // ヘッダ | 明細
    private System.Windows.Forms.SplitContainer splitDebug  = null!; // 上(DDX state) | 下(Op log)

    // ── IDD_SUPPLIER_INFO ─────────────────────────────────────────────────
    private System.Windows.Forms.GroupBox grpSupplier            = null!;
    private System.Windows.Forms.Label   lblSupplierCode         = null!;
    private System.Windows.Forms.TextBox IDC_EDIT_SUPPLIER_CODE  = null!;
    private System.Windows.Forms.Label   lblSupplierName         = null!;
    private System.Windows.Forms.TextBox IDC_EDIT_SUPPLIER_NAME  = null!;
    private System.Windows.Forms.Label   lblAddress              = null!;
    private System.Windows.Forms.TextBox IDC_EDIT_ADDRESS        = null!;
    private System.Windows.Forms.Label   lblTel                  = null!;
    private System.Windows.Forms.TextBox IDC_EDIT_TEL            = null!;
    private System.Windows.Forms.Label   lblFax                  = null!;
    private System.Windows.Forms.TextBox IDC_EDIT_FAX            = null!;

    // ── IDD_ORDER_HEADER ──────────────────────────────────────────────────
    private System.Windows.Forms.GroupBox grpOrderHeader          = null!;
    private System.Windows.Forms.Label    lblOrderNo              = null!;
    private System.Windows.Forms.TextBox  IDC_EDIT_ORDER_NO       = null!;
    private System.Windows.Forms.Label    lblOrderDate            = null!;
    private System.Windows.Forms.TextBox  IDC_EDIT_ORDER_DATE     = null!;
    private System.Windows.Forms.Label    lblDeliveryDate         = null!;
    private System.Windows.Forms.TextBox  IDC_EDIT_DELIVERY_DATE  = null!;
    private System.Windows.Forms.Label    lblStatus               = null!;
    private System.Windows.Forms.ComboBox IDC_COMBO_STATUS        = null!;
    private System.Windows.Forms.CheckBox IDC_CHECK_URGENT        = null!;
    private System.Windows.Forms.Label    lblMemo                 = null!;
    private System.Windows.Forms.TextBox  IDC_EDIT_MEMO           = null!;

    // ── IDD_ORDER_DETAIL ──────────────────────────────────────────────────
    private System.Windows.Forms.GroupBox     grpOrderDetail       = null!;
    private System.Windows.Forms.Panel        pnlOrderDetailFooter = null!;
    private System.Windows.Forms.DataGridView IDC_GRID_LINES       = null!;
    private System.Windows.Forms.Label        lblTotalCaption      = null!;
    private System.Windows.Forms.Label        IDC_LABEL_TOTAL      = null!;

    // ── Bottom button panel ───────────────────────────────────────────────
    private System.Windows.Forms.Panel  pnlButtons = null!;
    private System.Windows.Forms.Button btnNew     = null!;
    private System.Windows.Forms.Button btnSave    = null!;
    private System.Windows.Forms.Button btnCancel  = null!;

    // ── Right pane: debug panel ───────────────────────────────────────────
    private System.Windows.Forms.GroupBox     grpDebug          = null!;
    private System.Windows.Forms.GroupBox     grpDdxState       = null!;
    private System.Windows.Forms.GroupBox     grpOpLog          = null!;
    private System.Windows.Forms.DataGridView dgvDdxState       = null!;
    private System.Windows.Forms.RichTextBox  rtbOpLog          = null!;
    private System.Windows.Forms.Panel        pnlOpButtons      = null!;
    private System.Windows.Forms.Button       btnClearLog       = null!;
    private System.Windows.Forms.Button       btnRefreshState   = null!;
    private System.Windows.Forms.Button       btnRunUpdateFalse = null!;
    private System.Windows.Forms.Button       btnRunUpdateTrue  = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
            components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        menuStrip1 = new MenuStrip();
        menuFile = new ToolStripMenuItem();
        menuFileNew = new ToolStripMenuItem();
        menuFileOpen = new ToolStripMenuItem();
        menuFileSep1 = new ToolStripSeparator();
        menuFileSave = new ToolStripMenuItem();
        menuFileSep2 = new ToolStripSeparator();
        menuFileExit = new ToolStripMenuItem();
        pnlButtons = new Panel();
        btnNew = new Button();
        btnSave = new Button();
        btnCancel = new Button();
        splitMain = new SplitContainer();
        splitLeft1 = new SplitContainer();
        grpSupplier = new GroupBox();
        lblSupplierCode = new Label();
        IDC_EDIT_SUPPLIER_CODE = new TextBox();
        lblSupplierName = new Label();
        IDC_EDIT_SUPPLIER_NAME = new TextBox();
        lblAddress = new Label();
        IDC_EDIT_ADDRESS = new TextBox();
        lblTel = new Label();
        IDC_EDIT_TEL = new TextBox();
        lblFax = new Label();
        IDC_EDIT_FAX = new TextBox();
        splitLeft2 = new SplitContainer();
        grpOrderHeader = new GroupBox();
        lblOrderNo = new Label();
        IDC_EDIT_ORDER_NO = new TextBox();
        lblOrderDate = new Label();
        IDC_EDIT_ORDER_DATE = new TextBox();
        lblDeliveryDate = new Label();
        IDC_EDIT_DELIVERY_DATE = new TextBox();
        lblStatus = new Label();
        IDC_COMBO_STATUS = new ComboBox();
        IDC_CHECK_URGENT = new CheckBox();
        lblMemo = new Label();
        IDC_EDIT_MEMO = new TextBox();
        grpOrderDetail = new GroupBox();
        IDC_GRID_LINES = new DataGridView();
        pnlOrderDetailFooter = new Panel();
        lblTotalCaption = new Label();
        IDC_LABEL_TOTAL = new Label();
        grpDebug = new GroupBox();
        splitDebug = new SplitContainer();
        grpDdxState = new GroupBox();
        dgvDdxState = new DataGridView();
        grpOpLog = new GroupBox();
        rtbOpLog = new RichTextBox();
        pnlOpButtons = new Panel();
        btnClearLog = new Button();
        btnRefreshState = new Button();
        btnRunUpdateFalse = new Button();
        btnRunUpdateTrue = new Button();
        menuStrip1.SuspendLayout();
        pnlButtons.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitMain).BeginInit();
        splitMain.Panel1.SuspendLayout();
        splitMain.Panel2.SuspendLayout();
        splitMain.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitLeft1).BeginInit();
        splitLeft1.Panel1.SuspendLayout();
        splitLeft1.Panel2.SuspendLayout();
        splitLeft1.SuspendLayout();
        grpSupplier.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitLeft2).BeginInit();
        splitLeft2.Panel1.SuspendLayout();
        splitLeft2.Panel2.SuspendLayout();
        splitLeft2.SuspendLayout();
        grpOrderHeader.SuspendLayout();
        grpOrderDetail.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)IDC_GRID_LINES).BeginInit();
        pnlOrderDetailFooter.SuspendLayout();
        grpDebug.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitDebug).BeginInit();
        splitDebug.Panel1.SuspendLayout();
        splitDebug.Panel2.SuspendLayout();
        splitDebug.SuspendLayout();
        grpDdxState.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvDdxState).BeginInit();
        grpOpLog.SuspendLayout();
        pnlOpButtons.SuspendLayout();
        SuspendLayout();
        // 
        // menuStrip1
        // 
        menuStrip1.ImageScalingSize = new Size(32, 32);
        menuStrip1.Items.AddRange(new ToolStripItem[] { menuFile });
        menuStrip1.Location = new Point(0, 0);
        menuStrip1.Name = "menuStrip1";
        menuStrip1.Size = new Size(1440, 40);
        menuStrip1.TabIndex = 0;
        // 
        // menuFile
        // 
        menuFile.DropDownItems.AddRange(new ToolStripItem[] { menuFileNew, menuFileOpen, menuFileSep1, menuFileSave, menuFileSep2, menuFileExit });
        menuFile.Name = "menuFile";
        menuFile.Size = new Size(71, 36);
        menuFile.Text = "&File";
        // 
        // menuFileNew
        // 
        menuFileNew.Name = "menuFileNew";
        menuFileNew.Size = new Size(221, 44);
        menuFileNew.Text = "&New";
        menuFileNew.Click += menuFileNew_Click;
        // 
        // menuFileOpen
        // 
        menuFileOpen.Name = "menuFileOpen";
        menuFileOpen.Size = new Size(221, 44);
        menuFileOpen.Text = "&Open...";
        menuFileOpen.Click += menuFileOpen_Click;
        // 
        // menuFileSep1
        // 
        menuFileSep1.Name = "menuFileSep1";
        menuFileSep1.Size = new Size(218, 6);
        // 
        // menuFileSave
        // 
        menuFileSave.Name = "menuFileSave";
        menuFileSave.Size = new Size(221, 44);
        menuFileSave.Text = "&Save";
        menuFileSave.Click += menuFileSave_Click;
        // 
        // menuFileSep2
        // 
        menuFileSep2.Name = "menuFileSep2";
        menuFileSep2.Size = new Size(218, 6);
        // 
        // menuFileExit
        // 
        menuFileExit.Name = "menuFileExit";
        menuFileExit.Size = new Size(221, 44);
        menuFileExit.Text = "E&xit";
        menuFileExit.Click += menuFileExit_Click;
        // 
        // pnlButtons
        // 
        pnlButtons.Controls.Add(btnNew);
        pnlButtons.Controls.Add(btnSave);
        pnlButtons.Controls.Add(btnCancel);
        pnlButtons.Dock = DockStyle.Bottom;
        pnlButtons.Location = new Point(0, 900);
        pnlButtons.Name = "pnlButtons";
        pnlButtons.Padding = new Padding(16, 12, 16, 12);
        pnlButtons.Size = new Size(1440, 60);
        pnlButtons.TabIndex = 1;
        // 
        // btnNew
        // 
        btnNew.Location = new Point(6, 14);
        btnNew.Name = "btnNew";
        btnNew.Size = new Size(96, 32);
        btnNew.TabIndex = 0;
        btnNew.Text = "&New";
        btnNew.Click += btnNew_Click;
        // 
        // btnSave
        // 
        btnSave.Location = new Point(114, 14);
        btnSave.Name = "btnSave";
        btnSave.Size = new Size(96, 32);
        btnSave.TabIndex = 1;
        btnSave.Text = "&Save";
        btnSave.Click += btnSave_Click;
        // 
        // btnCancel
        // 
        btnCancel.Location = new Point(222, 14);
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(96, 32);
        btnCancel.TabIndex = 2;
        btnCancel.Text = "Cancel";
        btnCancel.Click += btnCancel_Click;
        // 
        // splitMain
        // 
        splitMain.Dock = DockStyle.Fill;
        splitMain.Location = new Point(0, 40);
        splitMain.Name = "splitMain";
        // 
        // splitMain.Panel1
        // 
        splitMain.Panel1.Controls.Add(splitLeft1);
        splitMain.Panel1MinSize = 50;
        // 
        // splitMain.Panel2
        // 
        splitMain.Panel2.Controls.Add(grpDebug);
        splitMain.Panel2MinSize = 50;
        splitMain.Size = new Size(1440, 860);
        splitMain.SplitterDistance = 480;
        splitMain.TabIndex = 2;
        // 
        // splitLeft1
        // 
        splitLeft1.Dock = DockStyle.Fill;
        splitLeft1.Location = new Point(0, 0);
        splitLeft1.Name = "splitLeft1";
        splitLeft1.Orientation = Orientation.Horizontal;
        // 
        // splitLeft1.Panel1
        // 
        splitLeft1.Panel1.Controls.Add(grpSupplier);
        splitLeft1.Panel1MinSize = 120;
        // 
        // splitLeft1.Panel2
        // 
        splitLeft1.Panel2.Controls.Add(splitLeft2);
        splitLeft1.Panel2MinSize = 80;
        splitLeft1.Size = new Size(480, 860);
        splitLeft1.SplitterDistance = 610;
        splitLeft1.TabIndex = 0;
        // 
        // grpSupplier
        // 
        grpSupplier.Controls.Add(lblSupplierCode);
        grpSupplier.Controls.Add(IDC_EDIT_SUPPLIER_CODE);
        grpSupplier.Controls.Add(lblSupplierName);
        grpSupplier.Controls.Add(IDC_EDIT_SUPPLIER_NAME);
        grpSupplier.Controls.Add(lblAddress);
        grpSupplier.Controls.Add(IDC_EDIT_ADDRESS);
        grpSupplier.Controls.Add(lblTel);
        grpSupplier.Controls.Add(IDC_EDIT_TEL);
        grpSupplier.Controls.Add(lblFax);
        grpSupplier.Controls.Add(IDC_EDIT_FAX);
        grpSupplier.Dock = DockStyle.Fill;
        grpSupplier.Location = new Point(0, 0);
        grpSupplier.Name = "grpSupplier";
        grpSupplier.Padding = new Padding(16, 18, 16, 20);
        grpSupplier.Size = new Size(480, 610);
        grpSupplier.TabIndex = 0;
        grpSupplier.TabStop = false;
        grpSupplier.Text = "Supplier information";
        // 
        // lblSupplierCode
        // 
        lblSupplierCode.Location = new Point(12, 40);
        lblSupplierCode.Name = "lblSupplierCode";
        lblSupplierCode.Size = new Size(132, 28);
        lblSupplierCode.TabIndex = 0;
        lblSupplierCode.Text = "Supplier code";
        lblSupplierCode.TextAlign = ContentAlignment.MiddleRight;
        // 
        // IDC_EDIT_SUPPLIER_CODE
        // 
        IDC_EDIT_SUPPLIER_CODE.Location = new Point(156, 42);
        IDC_EDIT_SUPPLIER_CODE.MaxLength = 10;
        IDC_EDIT_SUPPLIER_CODE.Name = "IDC_EDIT_SUPPLIER_CODE";
        IDC_EDIT_SUPPLIER_CODE.Size = new Size(140, 39);
        IDC_EDIT_SUPPLIER_CODE.TabIndex = 1;
        // 
        // lblSupplierName
        // 
        lblSupplierName.Location = new Point(12, 92);
        lblSupplierName.Name = "lblSupplierName";
        lblSupplierName.Size = new Size(132, 28);
        lblSupplierName.TabIndex = 2;
        lblSupplierName.Text = "Supplier name";
        lblSupplierName.TextAlign = ContentAlignment.MiddleRight;
        // 
        // IDC_EDIT_SUPPLIER_NAME
        // 
        IDC_EDIT_SUPPLIER_NAME.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        IDC_EDIT_SUPPLIER_NAME.Location = new Point(156, 94);
        IDC_EDIT_SUPPLIER_NAME.MaxLength = 50;
        IDC_EDIT_SUPPLIER_NAME.Name = "IDC_EDIT_SUPPLIER_NAME";
        IDC_EDIT_SUPPLIER_NAME.Size = new Size(600, 39);
        IDC_EDIT_SUPPLIER_NAME.TabIndex = 3;
        // 
        // lblAddress
        // 
        lblAddress.Location = new Point(12, 144);
        lblAddress.Name = "lblAddress";
        lblAddress.Size = new Size(132, 28);
        lblAddress.TabIndex = 4;
        lblAddress.Text = "Address";
        lblAddress.TextAlign = ContentAlignment.MiddleRight;
        // 
        // IDC_EDIT_ADDRESS
        // 
        IDC_EDIT_ADDRESS.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        IDC_EDIT_ADDRESS.Location = new Point(156, 146);
        IDC_EDIT_ADDRESS.MaxLength = 100;
        IDC_EDIT_ADDRESS.Name = "IDC_EDIT_ADDRESS";
        IDC_EDIT_ADDRESS.Size = new Size(600, 39);
        IDC_EDIT_ADDRESS.TabIndex = 5;
        // 
        // lblTel
        // 
        lblTel.Location = new Point(12, 196);
        lblTel.Name = "lblTel";
        lblTel.Size = new Size(132, 28);
        lblTel.TabIndex = 6;
        lblTel.Text = "Phone";
        lblTel.TextAlign = ContentAlignment.MiddleRight;
        // 
        // IDC_EDIT_TEL
        // 
        IDC_EDIT_TEL.Location = new Point(156, 198);
        IDC_EDIT_TEL.MaxLength = 20;
        IDC_EDIT_TEL.Name = "IDC_EDIT_TEL";
        IDC_EDIT_TEL.Size = new Size(210, 39);
        IDC_EDIT_TEL.TabIndex = 7;
        // 
        // lblFax
        // 
        lblFax.Location = new Point(12, 248);
        lblFax.Name = "lblFax";
        lblFax.Size = new Size(132, 28);
        lblFax.TabIndex = 8;
        lblFax.Text = "Fax";
        lblFax.TextAlign = ContentAlignment.MiddleRight;
        // 
        // IDC_EDIT_FAX
        // 
        IDC_EDIT_FAX.Location = new Point(156, 250);
        IDC_EDIT_FAX.MaxLength = 20;
        IDC_EDIT_FAX.Name = "IDC_EDIT_FAX";
        IDC_EDIT_FAX.Size = new Size(210, 39);
        IDC_EDIT_FAX.TabIndex = 9;
        // 
        // splitLeft2
        // 
        splitLeft2.Dock = DockStyle.Fill;
        splitLeft2.Location = new Point(0, 0);
        splitLeft2.Name = "splitLeft2";
        splitLeft2.Orientation = Orientation.Horizontal;
        // 
        // splitLeft2.Panel1
        // 
        splitLeft2.Panel1.Controls.Add(grpOrderHeader);
        splitLeft2.Panel1MinSize = 120;
        // 
        // splitLeft2.Panel2
        // 
        splitLeft2.Panel2.Controls.Add(grpOrderDetail);
        splitLeft2.Panel2MinSize = 80;
        splitLeft2.Size = new Size(480, 246);
        splitLeft2.SplitterDistance = 162;
        splitLeft2.TabIndex = 0;
        // 
        // grpOrderHeader
        // 
        grpOrderHeader.Controls.Add(lblOrderNo);
        grpOrderHeader.Controls.Add(IDC_EDIT_ORDER_NO);
        grpOrderHeader.Controls.Add(lblOrderDate);
        grpOrderHeader.Controls.Add(IDC_EDIT_ORDER_DATE);
        grpOrderHeader.Controls.Add(lblDeliveryDate);
        grpOrderHeader.Controls.Add(IDC_EDIT_DELIVERY_DATE);
        grpOrderHeader.Controls.Add(lblStatus);
        grpOrderHeader.Controls.Add(IDC_COMBO_STATUS);
        grpOrderHeader.Controls.Add(IDC_CHECK_URGENT);
        grpOrderHeader.Controls.Add(lblMemo);
        grpOrderHeader.Controls.Add(IDC_EDIT_MEMO);
        grpOrderHeader.Dock = DockStyle.Fill;
        grpOrderHeader.Location = new Point(0, 0);
        grpOrderHeader.Name = "grpOrderHeader";
        grpOrderHeader.Padding = new Padding(16, 18, 16, 18);
        grpOrderHeader.Size = new Size(480, 162);
        grpOrderHeader.TabIndex = 0;
        grpOrderHeader.TabStop = false;
        grpOrderHeader.Text = "Order header";
        // 
        // lblOrderNo
        // 
        lblOrderNo.Location = new Point(12, 40);
        lblOrderNo.Name = "lblOrderNo";
        lblOrderNo.Size = new Size(132, 28);
        lblOrderNo.TabIndex = 0;
        lblOrderNo.Text = "Order no.";
        lblOrderNo.TextAlign = ContentAlignment.MiddleRight;
        // 
        // IDC_EDIT_ORDER_NO
        // 
        IDC_EDIT_ORDER_NO.BackColor = SystemColors.Info;
        IDC_EDIT_ORDER_NO.Location = new Point(156, 42);
        IDC_EDIT_ORDER_NO.Name = "IDC_EDIT_ORDER_NO";
        IDC_EDIT_ORDER_NO.ReadOnly = true;
        IDC_EDIT_ORDER_NO.Size = new Size(220, 39);
        IDC_EDIT_ORDER_NO.TabIndex = 1;
        // 
        // lblOrderDate
        // 
        lblOrderDate.Location = new Point(12, 92);
        lblOrderDate.Name = "lblOrderDate";
        lblOrderDate.Size = new Size(132, 28);
        lblOrderDate.TabIndex = 2;
        lblOrderDate.Text = "Order date";
        lblOrderDate.TextAlign = ContentAlignment.MiddleRight;
        // 
        // IDC_EDIT_ORDER_DATE
        // 
        IDC_EDIT_ORDER_DATE.Location = new Point(156, 94);
        IDC_EDIT_ORDER_DATE.Name = "IDC_EDIT_ORDER_DATE";
        IDC_EDIT_ORDER_DATE.Size = new Size(140, 39);
        IDC_EDIT_ORDER_DATE.TabIndex = 3;
        // 
        // lblDeliveryDate
        // 
        lblDeliveryDate.Location = new Point(12, 144);
        lblDeliveryDate.Name = "lblDeliveryDate";
        lblDeliveryDate.Size = new Size(132, 28);
        lblDeliveryDate.TabIndex = 4;
        lblDeliveryDate.Text = "Delivery date";
        lblDeliveryDate.TextAlign = ContentAlignment.MiddleRight;
        // 
        // IDC_EDIT_DELIVERY_DATE
        // 
        IDC_EDIT_DELIVERY_DATE.Location = new Point(156, 146);
        IDC_EDIT_DELIVERY_DATE.Name = "IDC_EDIT_DELIVERY_DATE";
        IDC_EDIT_DELIVERY_DATE.Size = new Size(140, 39);
        IDC_EDIT_DELIVERY_DATE.TabIndex = 5;
        // 
        // lblStatus
        // 
        lblStatus.Location = new Point(12, 196);
        lblStatus.Name = "lblStatus";
        lblStatus.Size = new Size(132, 28);
        lblStatus.TabIndex = 6;
        lblStatus.Text = "Status";
        lblStatus.TextAlign = ContentAlignment.MiddleRight;
        // 
        // IDC_COMBO_STATUS
        // 
        IDC_COMBO_STATUS.DropDownStyle = ComboBoxStyle.DropDownList;
        IDC_COMBO_STATUS.Location = new Point(156, 198);
        IDC_COMBO_STATUS.Name = "IDC_COMBO_STATUS";
        IDC_COMBO_STATUS.Size = new Size(180, 40);
        IDC_COMBO_STATUS.TabIndex = 7;
        // 
        // IDC_CHECK_URGENT
        // 
        IDC_CHECK_URGENT.AutoSize = true;
        IDC_CHECK_URGENT.Location = new Point(348, 202);
        IDC_CHECK_URGENT.Name = "IDC_CHECK_URGENT";
        IDC_CHECK_URGENT.Size = new Size(119, 36);
        IDC_CHECK_URGENT.TabIndex = 8;
        IDC_CHECK_URGENT.Text = "Urgent";
        // 
        // lblMemo
        // 
        lblMemo.Location = new Point(12, 248);
        lblMemo.Name = "lblMemo";
        lblMemo.Size = new Size(132, 28);
        lblMemo.TabIndex = 9;
        lblMemo.Text = "Remarks";
        lblMemo.TextAlign = ContentAlignment.MiddleRight;
        // 
        // IDC_EDIT_MEMO
        // 
        IDC_EDIT_MEMO.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        IDC_EDIT_MEMO.Location = new Point(156, 250);
        IDC_EDIT_MEMO.Multiline = true;
        IDC_EDIT_MEMO.Name = "IDC_EDIT_MEMO";
        IDC_EDIT_MEMO.ScrollBars = ScrollBars.Vertical;
        IDC_EDIT_MEMO.Size = new Size(600, 158);
        IDC_EDIT_MEMO.TabIndex = 10;
        // 
        // grpOrderDetail
        // 
        grpOrderDetail.Controls.Add(IDC_GRID_LINES);
        grpOrderDetail.Controls.Add(pnlOrderDetailFooter);
        grpOrderDetail.Dock = DockStyle.Fill;
        grpOrderDetail.Location = new Point(0, 0);
        grpOrderDetail.Name = "grpOrderDetail";
        grpOrderDetail.Padding = new Padding(16, 18, 16, 14);
        grpOrderDetail.Size = new Size(480, 80);
        grpOrderDetail.TabIndex = 0;
        grpOrderDetail.TabStop = false;
        grpOrderDetail.Text = "Order details";
        // 
        // IDC_GRID_LINES
        // 
        IDC_GRID_LINES.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        IDC_GRID_LINES.Dock = DockStyle.Fill;
        IDC_GRID_LINES.Location = new Point(16, 50);
        IDC_GRID_LINES.Margin = new Padding(0, 10, 0, 10);
        IDC_GRID_LINES.Name = "IDC_GRID_LINES";
        IDC_GRID_LINES.RowHeadersWidth = 82;
        IDC_GRID_LINES.RowTemplate.Height = 26;
        IDC_GRID_LINES.Size = new Size(448, 0);
        IDC_GRID_LINES.TabIndex = 0;
        // 
        // pnlOrderDetailFooter
        // 
        pnlOrderDetailFooter.Controls.Add(lblTotalCaption);
        pnlOrderDetailFooter.Controls.Add(IDC_LABEL_TOTAL);
        pnlOrderDetailFooter.Dock = DockStyle.Bottom;
        pnlOrderDetailFooter.Location = new Point(16, 18);
        pnlOrderDetailFooter.Name = "pnlOrderDetailFooter";
        pnlOrderDetailFooter.Padding = new Padding(0, 12, 0, 6);
        pnlOrderDetailFooter.Size = new Size(448, 48);
        pnlOrderDetailFooter.TabIndex = 1;
        // 
        // lblTotalCaption
        // 
        lblTotalCaption.AutoSize = true;
        lblTotalCaption.Location = new Point(0, 14);
        lblTotalCaption.Name = "lblTotalCaption";
        lblTotalCaption.Size = new Size(154, 32);
        lblTotalCaption.TabIndex = 0;
        lblTotalCaption.Text = "Total amount";
        // 
        // IDC_LABEL_TOTAL
        // 
        IDC_LABEL_TOTAL.AutoSize = true;
        IDC_LABEL_TOTAL.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        IDC_LABEL_TOTAL.Location = new Point(100, 14);
        IDC_LABEL_TOTAL.Name = "IDC_LABEL_TOTAL";
        IDC_LABEL_TOTAL.Size = new Size(42, 32);
        IDC_LABEL_TOTAL.TabIndex = 1;
        IDC_LABEL_TOTAL.Text = "¥0";
        // 
        // grpDebug
        // 
        grpDebug.Controls.Add(splitDebug);
        grpDebug.Dock = DockStyle.Fill;
        grpDebug.Location = new Point(0, 0);
        grpDebug.Name = "grpDebug";
        grpDebug.Padding = new Padding(10);
        grpDebug.Size = new Size(956, 860);
        grpDebug.TabIndex = 0;
        grpDebug.TabStop = false;
        grpDebug.Text = "Debug panel";
        // 
        // splitDebug
        // 
        splitDebug.Dock = DockStyle.Fill;
        splitDebug.Location = new Point(10, 42);
        splitDebug.Name = "splitDebug";
        splitDebug.Orientation = Orientation.Horizontal;
        // 
        // splitDebug.Panel1
        // 
        splitDebug.Panel1.Controls.Add(grpDdxState);
        splitDebug.Panel1MinSize = 50;
        // 
        // splitDebug.Panel2
        // 
        splitDebug.Panel2.Controls.Add(grpOpLog);
        splitDebug.Panel2MinSize = 50;
        splitDebug.Size = new Size(936, 808);
        splitDebug.SplitterDistance = 404;
        splitDebug.TabIndex = 0;
        // 
        // grpDdxState
        // 
        grpDdxState.Controls.Add(dgvDdxState);
        grpDdxState.Dock = DockStyle.Fill;
        grpDdxState.Location = new Point(0, 0);
        grpDdxState.Name = "grpDdxState";
        grpDdxState.Padding = new Padding(8);
        grpDdxState.Size = new Size(936, 404);
        grpDdxState.TabIndex = 0;
        grpDdxState.TabStop = false;
        grpDdxState.Text = "DDX binding state";
        // 
        // dgvDdxState
        // 
        dgvDdxState.AllowUserToAddRows = false;
        dgvDdxState.AllowUserToDeleteRows = false;
        dgvDdxState.AllowUserToResizeRows = false;
        dgvDdxState.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvDdxState.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvDdxState.Dock = DockStyle.Fill;
        dgvDdxState.Location = new Point(8, 40);
        dgvDdxState.Name = "dgvDdxState";
        dgvDdxState.ReadOnly = true;
        dgvDdxState.RowHeadersVisible = false;
        dgvDdxState.RowHeadersWidth = 82;
        dgvDdxState.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvDdxState.Size = new Size(920, 356);
        dgvDdxState.TabIndex = 0;
        dgvDdxState.TabStop = false;
        // 
        // grpOpLog
        // 
        grpOpLog.Controls.Add(rtbOpLog);
        grpOpLog.Controls.Add(pnlOpButtons);
        grpOpLog.Dock = DockStyle.Fill;
        grpOpLog.Location = new Point(0, 0);
        grpOpLog.Name = "grpOpLog";
        grpOpLog.Padding = new Padding(8);
        grpOpLog.Size = new Size(936, 400);
        grpOpLog.TabIndex = 0;
        grpOpLog.TabStop = false;
        grpOpLog.Text = "Operation log (DDX/DDV)";
        // 
        // rtbOpLog
        // 
        rtbOpLog.BackColor = Color.FromArgb(30, 30, 30);
        rtbOpLog.Dock = DockStyle.Fill;
        rtbOpLog.Font = new Font("Consolas", 8.25F);
        rtbOpLog.ForeColor = Color.LightGray;
        rtbOpLog.Location = new Point(8, 40);
        rtbOpLog.Name = "rtbOpLog";
        rtbOpLog.ReadOnly = true;
        rtbOpLog.ScrollBars = RichTextBoxScrollBars.Vertical;
        rtbOpLog.Size = new Size(920, 296);
        rtbOpLog.TabIndex = 0;
        rtbOpLog.TabStop = false;
        rtbOpLog.Text = "";
        rtbOpLog.WordWrap = false;
        // 
        // pnlOpButtons
        // 
        pnlOpButtons.Controls.Add(btnClearLog);
        pnlOpButtons.Controls.Add(btnRefreshState);
        pnlOpButtons.Controls.Add(btnRunUpdateFalse);
        pnlOpButtons.Controls.Add(btnRunUpdateTrue);
        pnlOpButtons.Dock = DockStyle.Bottom;
        pnlOpButtons.Location = new Point(8, 336);
        pnlOpButtons.Name = "pnlOpButtons";
        pnlOpButtons.Padding = new Padding(0, 6, 0, 6);
        pnlOpButtons.Size = new Size(920, 56);
        pnlOpButtons.TabIndex = 1;
        // 
        // btnClearLog
        // 
        btnClearLog.Location = new Point(4, 10);
        btnClearLog.Name = "btnClearLog";
        btnClearLog.Size = new Size(100, 32);
        btnClearLog.TabIndex = 50;
        btnClearLog.Text = "Clear log";
        btnClearLog.Click += btnClearLog_Click;
        // 
        // btnRefreshState
        // 
        btnRefreshState.Location = new Point(112, 10);
        btnRefreshState.Name = "btnRefreshState";
        btnRefreshState.Size = new Size(110, 32);
        btnRefreshState.TabIndex = 51;
        btnRefreshState.Text = "Refresh state";
        btnRefreshState.Click += btnRefreshState_Click;
        // 
        // btnRunUpdateFalse
        // 
        btnRunUpdateFalse.Location = new Point(230, 10);
        btnRunUpdateFalse.Name = "btnRunUpdateFalse";
        btnRunUpdateFalse.Size = new Size(110, 32);
        btnRunUpdateFalse.TabIndex = 52;
        btnRunUpdateFalse.Text = "▶ Doc→UI";
        btnRunUpdateFalse.Click += btnRunUpdateFalse_Click;
        // 
        // btnRunUpdateTrue
        // 
        btnRunUpdateTrue.Location = new Point(348, 10);
        btnRunUpdateTrue.Name = "btnRunUpdateTrue";
        btnRunUpdateTrue.Size = new Size(130, 32);
        btnRunUpdateTrue.TabIndex = 53;
        btnRunUpdateTrue.Text = "▶ UI→Doc+DDV";
        btnRunUpdateTrue.Click += btnRunUpdateTrue_Click;
        // 
        // OrderView
        // 
        ClientSize = new Size(1440, 960);
        Controls.Add(splitMain);
        Controls.Add(pnlButtons);
        Controls.Add(menuStrip1);
        MainMenuStrip = menuStrip1;
        MinimumSize = new Size(720, 520);
        Name = "OrderView";
        Text = "Order management";
        menuStrip1.ResumeLayout(false);
        menuStrip1.PerformLayout();
        pnlButtons.ResumeLayout(false);
        splitMain.Panel1.ResumeLayout(false);
        splitMain.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)splitMain).EndInit();
        splitMain.ResumeLayout(false);
        splitLeft1.Panel1.ResumeLayout(false);
        splitLeft1.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)splitLeft1).EndInit();
        splitLeft1.ResumeLayout(false);
        grpSupplier.ResumeLayout(false);
        grpSupplier.PerformLayout();
        splitLeft2.Panel1.ResumeLayout(false);
        splitLeft2.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)splitLeft2).EndInit();
        splitLeft2.ResumeLayout(false);
        grpOrderHeader.ResumeLayout(false);
        grpOrderHeader.PerformLayout();
        grpOrderDetail.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)IDC_GRID_LINES).EndInit();
        pnlOrderDetailFooter.ResumeLayout(false);
        pnlOrderDetailFooter.PerformLayout();
        grpDebug.ResumeLayout(false);
        splitDebug.Panel1.ResumeLayout(false);
        splitDebug.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)splitDebug).EndInit();
        splitDebug.ResumeLayout(false);
        grpDdxState.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)dgvDdxState).EndInit();
        grpOpLog.ResumeLayout(false);
        pnlOpButtons.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }
}
