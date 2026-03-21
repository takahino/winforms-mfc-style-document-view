namespace DocumentView.Sample;

partial class SampleView
{
    private System.ComponentModel.IContainer components = null!;

    // ── Menu ───────────────────────────────────────────────────────────────
    private System.Windows.Forms.MenuStrip        menuStrip1    = null!;
    private System.Windows.Forms.ToolStripMenuItem menuFile      = null!;
    private System.Windows.Forms.ToolStripMenuItem menuFileNew   = null!;
    private System.Windows.Forms.ToolStripMenuItem menuFileLoad  = null!;
    private System.Windows.Forms.ToolStripSeparator menuFileSep1 = null!;
    private System.Windows.Forms.ToolStripMenuItem menuFileSave  = null!;
    private System.Windows.Forms.ToolStripSeparator menuFileSep2 = null!;
    private System.Windows.Forms.ToolStripMenuItem menuFileExit  = null!;

    // ── Left pane: group boxes ─────────────────────────────────────────────
    private System.Windows.Forms.GroupBox grpBasicInfo = null!;
    private System.Windows.Forms.GroupBox grpWorkInfo  = null!;
    private System.Windows.Forms.GroupBox grpProjects  = null!;

    // ── Left pane: labels ─────────────────────────────────────────────────
    private System.Windows.Forms.Label lblEmpNo    = null!;
    private System.Windows.Forms.Label lblName     = null!;
    private System.Windows.Forms.Label lblAge      = null!;
    private System.Windows.Forms.Label lblMemo     = null!;
    private System.Windows.Forms.Label lblPref     = null!;
    private System.Windows.Forms.Label lblLang     = null!;
    private System.Windows.Forms.Label lblPriority = null!;
    private System.Windows.Forms.Label lblGender   = null!;
    private System.Windows.Forms.Label lblItems    = null!;

    // ── Left pane: inputs (field name = Name = IDC_*) ─────────────────────
    private System.Windows.Forms.TextBox        IDC_EDIT_EMPNO    = null!;
    private System.Windows.Forms.TextBox        IDC_EDIT_NAME     = null!;
    private System.Windows.Forms.TextBox        IDC_EDIT_AGE      = null!;
    private System.Windows.Forms.TextBox        IDC_EDIT_MEMO     = null!;
    private System.Windows.Forms.CheckBox       IDC_CHECK_ACTIVE  = null!;
    private System.Windows.Forms.CheckBox       IDC_CHECK_NOTIFY  = null!;
    private System.Windows.Forms.ComboBox       IDC_COMBO_PREF    = null!;
    private System.Windows.Forms.ComboBox       IDC_COMBO_LANG    = null!;
    private System.Windows.Forms.NumericUpDown  IDC_SPIN_PRIORITY = null!;
    private System.Windows.Forms.Panel          pnlGender         = null!;
    private System.Windows.Forms.RadioButton    IDC_RADIO_MALE    = null!;
    private System.Windows.Forms.RadioButton    IDC_RADIO_FEMALE  = null!;

    // ── Employment type ───────────────────────────────────────────────────
    private System.Windows.Forms.Label          lblEmployment         = null!;
    private System.Windows.Forms.Panel          pnlEmployment         = null!;
    private System.Windows.Forms.RadioButton    IDC_RADIO_FULL_TIME   = null!;
    private System.Windows.Forms.RadioButton    IDC_RADIO_CONTRACT    = null!;
    private System.Windows.Forms.RadioButton    IDC_RADIO_PART_TIME   = null!;
    private System.Windows.Forms.ListBox        IDC_LIST_ITEMS    = null!;
    private System.Windows.Forms.Label          IDC_STATIC_STATUS = null!;
    private System.Windows.Forms.DataGridView   IDC_GRID_ITEMS    = null!;

    // ── Left pane: buttons ────────────────────────────────────────────────
    private System.Windows.Forms.Button btnOk     = null!;
    private System.Windows.Forms.Button btnCancel = null!;
    private System.Windows.Forms.Button btnNew    = null!;

    // ── Right pane: debug panel ───────────────────────────────────────────
    private System.Windows.Forms.GroupBox     grpDebug       = null!;
    private System.Windows.Forms.GroupBox     grpDdxState    = null!;
    private System.Windows.Forms.GroupBox     grpOpLog       = null!;
    private System.Windows.Forms.DataGridView dgvDdxState    = null!;
    private System.Windows.Forms.RichTextBox  rtbOpLog       = null!;
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
        menuFileLoad = new ToolStripMenuItem();
        menuFileSep1 = new ToolStripSeparator();
        menuFileSave = new ToolStripMenuItem();
        menuFileSep2 = new ToolStripSeparator();
        menuFileExit = new ToolStripMenuItem();
        grpBasicInfo = new GroupBox();
        lblEmpNo = new Label();
        IDC_EDIT_EMPNO = new TextBox();
        lblName = new Label();
        IDC_EDIT_NAME = new TextBox();
        lblAge = new Label();
        IDC_EDIT_AGE = new TextBox();
        lblGender = new Label();
        pnlGender = new Panel();
        checkBox1 = new CheckBox();
        IDC_RADIO_MALE = new RadioButton();
        IDC_RADIO_FEMALE = new RadioButton();
        lblMemo = new Label();
        IDC_EDIT_MEMO = new TextBox();
        grpWorkInfo = new GroupBox();
        IDC_CHECK_ACTIVE = new CheckBox();
        IDC_CHECK_NOTIFY = new CheckBox();
        lblPref = new Label();
        IDC_COMBO_PREF = new ComboBox();
        lblLang = new Label();
        IDC_COMBO_LANG = new ComboBox();
        lblPriority = new Label();
        IDC_SPIN_PRIORITY = new NumericUpDown();
        lblItems = new Label();
        IDC_LIST_ITEMS = new ListBox();
        lblEmployment = new Label();
        pnlEmployment = new Panel();
        IDC_RADIO_FULL_TIME = new RadioButton();
        IDC_RADIO_CONTRACT = new RadioButton();
        IDC_RADIO_PART_TIME = new RadioButton();
        grpProjects = new GroupBox();
        IDC_GRID_ITEMS = new DataGridView();
        IDC_STATIC_STATUS = new Label();
        btnOk = new Button();
        btnCancel = new Button();
        btnNew = new Button();
        grpDebug = new GroupBox();
        grpDdxState = new GroupBox();
        dgvDdxState = new DataGridView();
        grpOpLog = new GroupBox();
        rtbOpLog = new RichTextBox();
        btnClearLog = new Button();
        btnRefreshState = new Button();
        btnRunUpdateFalse = new Button();
        btnRunUpdateTrue = new Button();
        menuStrip1.SuspendLayout();
        grpBasicInfo.SuspendLayout();
        pnlGender.SuspendLayout();
        grpWorkInfo.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)IDC_SPIN_PRIORITY).BeginInit();
        pnlEmployment.SuspendLayout();
        grpProjects.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)IDC_GRID_ITEMS).BeginInit();
        grpDebug.SuspendLayout();
        grpDdxState.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvDdxState).BeginInit();
        grpOpLog.SuspendLayout();
        SuspendLayout();
        // 
        // menuStrip1
        // 
        menuStrip1.ImageScalingSize = new Size(32, 32);
        menuStrip1.Items.AddRange(new ToolStripItem[] { menuFile });
        menuStrip1.Location = new Point(0, 0);
        menuStrip1.Name = "menuStrip1";
        menuStrip1.Padding = new Padding(11, 4, 0, 4);
        menuStrip1.Size = new Size(1839, 44);
        menuStrip1.TabIndex = 0;
        // 
        // menuFile
        // 
        menuFile.DropDownItems.AddRange(new ToolStripItem[] { menuFileNew, menuFileLoad, menuFileSep1, menuFileSave, menuFileSep2, menuFileExit });
        menuFile.Name = "menuFile";
        menuFile.Size = new Size(71, 36);
        menuFile.Text = "&File";
        // 
        // menuFileNew
        // 
        menuFileNew.Name = "menuFileNew";
        menuFileNew.ShortcutKeys = Keys.Control | Keys.N;
        menuFileNew.Size = new Size(291, 44);
        menuFileNew.Text = "&New";
        menuFileNew.Click += menuFileNew_Click;
        // 
        // menuFileLoad
        // 
        menuFileLoad.Name = "menuFileLoad";
        menuFileLoad.ShortcutKeys = Keys.Control | Keys.O;
        menuFileLoad.Size = new Size(291, 44);
        menuFileLoad.Text = "&Open";
        menuFileLoad.Click += menuFileLoad_Click;
        // 
        // menuFileSep1
        // 
        menuFileSep1.Name = "menuFileSep1";
        menuFileSep1.Size = new Size(288, 6);
        // 
        // menuFileSave
        // 
        menuFileSave.Name = "menuFileSave";
        menuFileSave.ShortcutKeys = Keys.Control | Keys.S;
        menuFileSave.Size = new Size(291, 44);
        menuFileSave.Text = "&Save";
        menuFileSave.Click += menuFileSave_Click;
        // 
        // menuFileSep2
        // 
        menuFileSep2.Name = "menuFileSep2";
        menuFileSep2.Size = new Size(288, 6);
        // 
        // menuFileExit
        // 
        menuFileExit.Name = "menuFileExit";
        menuFileExit.Size = new Size(291, 44);
        menuFileExit.Text = "E&xit";
        menuFileExit.Click += menuFileExit_Click;
        // 
        // grpBasicInfo
        // 
        grpBasicInfo.Controls.Add(lblEmpNo);
        grpBasicInfo.Controls.Add(IDC_EDIT_EMPNO);
        grpBasicInfo.Controls.Add(lblName);
        grpBasicInfo.Controls.Add(IDC_EDIT_NAME);
        grpBasicInfo.Controls.Add(lblAge);
        grpBasicInfo.Controls.Add(IDC_EDIT_AGE);
        grpBasicInfo.Controls.Add(lblGender);
        grpBasicInfo.Controls.Add(pnlGender);
        grpBasicInfo.Controls.Add(lblMemo);
        grpBasicInfo.Controls.Add(IDC_EDIT_MEMO);
        grpBasicInfo.Location = new Point(19, 64);
        grpBasicInfo.Margin = new Padding(6);
        grpBasicInfo.Name = "grpBasicInfo";
        grpBasicInfo.Padding = new Padding(6);
        grpBasicInfo.Size = new Size(910, 459);
        grpBasicInfo.TabIndex = 1;
        grpBasicInfo.TabStop = false;
        grpBasicInfo.Text = "Basic information";
        // 
        // lblEmpNo
        // 
        lblEmpNo.AutoSize = true;
        lblEmpNo.Location = new Point(28, 53);
        lblEmpNo.Margin = new Padding(6, 0, 6, 0);
        lblEmpNo.Name = "lblEmpNo";
        lblEmpNo.Size = new Size(164, 32);
        lblEmpNo.TabIndex = 0;
        lblEmpNo.Text = "Employee no.:";
        // 
        // IDC_EDIT_EMPNO
        // 
        IDC_EDIT_EMPNO.Location = new Point(198, 47);
        IDC_EDIT_EMPNO.Margin = new Padding(6);
        IDC_EDIT_EMPNO.Name = "IDC_EDIT_EMPNO";
        IDC_EDIT_EMPNO.Size = new Size(219, 39);
        IDC_EDIT_EMPNO.TabIndex = 9;
        // 
        // lblName
        // 
        lblName.AutoSize = true;
        lblName.Location = new Point(28, 117);
        lblName.Margin = new Padding(6, 0, 6, 0);
        lblName.Name = "lblName";
        lblName.Size = new Size(83, 32);
        lblName.TabIndex = 10;
        lblName.Text = "Name:";
        // 
        // IDC_EDIT_NAME
        // 
        IDC_EDIT_NAME.Location = new Point(198, 111);
        IDC_EDIT_NAME.Margin = new Padding(6);
        IDC_EDIT_NAME.Name = "IDC_EDIT_NAME";
        IDC_EDIT_NAME.Size = new Size(424, 39);
        IDC_EDIT_NAME.TabIndex = 10;
        // 
        // lblAge
        // 
        lblAge.AutoSize = true;
        lblAge.Location = new Point(28, 181);
        lblAge.Margin = new Padding(6, 0, 6, 0);
        lblAge.Name = "lblAge";
        lblAge.Size = new Size(61, 32);
        lblAge.TabIndex = 11;
        lblAge.Text = "Age:";
        // 
        // IDC_EDIT_AGE
        // 
        IDC_EDIT_AGE.Location = new Point(198, 175);
        IDC_EDIT_AGE.Margin = new Padding(6);
        IDC_EDIT_AGE.Name = "IDC_EDIT_AGE";
        IDC_EDIT_AGE.Size = new Size(108, 39);
        IDC_EDIT_AGE.TabIndex = 11;
        // 
        // lblGender
        // 
        lblGender.AutoSize = true;
        lblGender.Location = new Point(28, 245);
        lblGender.Margin = new Padding(6, 0, 6, 0);
        lblGender.Name = "lblGender";
        lblGender.Size = new Size(97, 32);
        lblGender.TabIndex = 12;
        lblGender.Text = "Gender:";
        // 
        // pnlGender
        // 
        pnlGender.Controls.Add(checkBox1);
        pnlGender.Controls.Add(IDC_RADIO_MALE);
        pnlGender.Controls.Add(IDC_RADIO_FEMALE);
        pnlGender.Location = new Point(198, 235);
        pnlGender.Margin = new Padding(6);
        pnlGender.Name = "pnlGender";
        pnlGender.Size = new Size(371, 58);
        pnlGender.TabIndex = 13;
        // 
        // checkBox1
        // 
        checkBox1.AutoSize = true;
        checkBox1.Location = new Point(373, 27);
        checkBox1.Name = "checkBox1";
        checkBox1.Size = new Size(159, 36);
        checkBox1.TabIndex = 20;
        checkBox1.Text = "checkBox1";
        checkBox1.UseVisualStyleBackColor = true;
        // 
        // IDC_RADIO_MALE
        // 
        IDC_RADIO_MALE.AutoSize = true;
        IDC_RADIO_MALE.Checked = true;
        IDC_RADIO_MALE.Location = new Point(13, 6);
        IDC_RADIO_MALE.Margin = new Padding(6);
        IDC_RADIO_MALE.Name = "IDC_RADIO_MALE";
        IDC_RADIO_MALE.Size = new Size(98, 36);
        IDC_RADIO_MALE.TabIndex = 18;
        IDC_RADIO_MALE.TabStop = true;
        IDC_RADIO_MALE.Text = "Male";
        // 
        // IDC_RADIO_FEMALE
        // 
        IDC_RADIO_FEMALE.AutoSize = true;
        IDC_RADIO_FEMALE.Location = new Point(142, 6);
        IDC_RADIO_FEMALE.Margin = new Padding(6);
        IDC_RADIO_FEMALE.Name = "IDC_RADIO_FEMALE";
        IDC_RADIO_FEMALE.Size = new Size(121, 36);
        IDC_RADIO_FEMALE.TabIndex = 19;
        IDC_RADIO_FEMALE.Text = "Female";
        // 
        // lblMemo
        // 
        lblMemo.AutoSize = true;
        lblMemo.Location = new Point(28, 309);
        lblMemo.Margin = new Padding(6, 0, 6, 0);
        lblMemo.Name = "lblMemo";
        lblMemo.Size = new Size(82, 32);
        lblMemo.TabIndex = 14;
        lblMemo.Text = "Notes:";
        // 
        // IDC_EDIT_MEMO
        // 
        IDC_EDIT_MEMO.Location = new Point(198, 303);
        IDC_EDIT_MEMO.Margin = new Padding(6);
        IDC_EDIT_MEMO.Multiline = true;
        IDC_EDIT_MEMO.Name = "IDC_EDIT_MEMO";
        IDC_EDIT_MEMO.Size = new Size(685, 113);
        IDC_EDIT_MEMO.TabIndex = 12;
        // 
        // grpWorkInfo
        // 
        grpWorkInfo.Controls.Add(IDC_CHECK_ACTIVE);
        grpWorkInfo.Controls.Add(IDC_CHECK_NOTIFY);
        grpWorkInfo.Controls.Add(lblPref);
        grpWorkInfo.Controls.Add(IDC_COMBO_PREF);
        grpWorkInfo.Controls.Add(lblLang);
        grpWorkInfo.Controls.Add(IDC_COMBO_LANG);
        grpWorkInfo.Controls.Add(lblPriority);
        grpWorkInfo.Controls.Add(IDC_SPIN_PRIORITY);
        grpWorkInfo.Controls.Add(lblItems);
        grpWorkInfo.Controls.Add(IDC_LIST_ITEMS);
        grpWorkInfo.Controls.Add(lblEmployment);
        grpWorkInfo.Controls.Add(pnlEmployment);
        grpWorkInfo.Location = new Point(19, 544);
        grpWorkInfo.Margin = new Padding(6);
        grpWorkInfo.Name = "grpWorkInfo";
        grpWorkInfo.Padding = new Padding(6);
        grpWorkInfo.Size = new Size(910, 469);
        grpWorkInfo.TabIndex = 2;
        grpWorkInfo.TabStop = false;
        grpWorkInfo.Text = "Work information";
        // 
        // IDC_CHECK_ACTIVE
        // 
        IDC_CHECK_ACTIVE.AutoSize = true;
        IDC_CHECK_ACTIVE.Location = new Point(28, 53);
        IDC_CHECK_ACTIVE.Margin = new Padding(6);
        IDC_CHECK_ACTIVE.Name = "IDC_CHECK_ACTIVE";
        IDC_CHECK_ACTIVE.Size = new Size(224, 36);
        IDC_CHECK_ACTIVE.TabIndex = 13;
        IDC_CHECK_ACTIVE.Text = "Active employee";
        // 
        // IDC_CHECK_NOTIFY
        // 
        IDC_CHECK_NOTIFY.AutoSize = true;
        IDC_CHECK_NOTIFY.Location = new Point(258, 53);
        IDC_CHECK_NOTIFY.Margin = new Padding(6);
        IDC_CHECK_NOTIFY.Name = "IDC_CHECK_NOTIFY";
        IDC_CHECK_NOTIFY.Size = new Size(240, 36);
        IDC_CHECK_NOTIFY.TabIndex = 14;
        IDC_CHECK_NOTIFY.Text = "Email notifications";
        // 
        // lblPref
        // 
        lblPref.AutoSize = true;
        lblPref.Location = new Point(28, 128);
        lblPref.Margin = new Padding(6, 0, 6, 0);
        lblPref.Name = "lblPref";
        lblPref.Size = new Size(109, 32);
        lblPref.TabIndex = 15;
        lblPref.Text = "Location:";
        // 
        // IDC_COMBO_PREF
        // 
        IDC_COMBO_PREF.DropDownStyle = ComboBoxStyle.DropDownList;
        IDC_COMBO_PREF.Location = new Point(247, 122);
        IDC_COMBO_PREF.Margin = new Padding(6);
        IDC_COMBO_PREF.Name = "IDC_COMBO_PREF";
        IDC_COMBO_PREF.Size = new Size(261, 40);
        IDC_COMBO_PREF.TabIndex = 15;
        // 
        // lblLang
        // 
        lblLang.AutoSize = true;
        lblLang.Location = new Point(28, 203);
        lblLang.Margin = new Padding(6, 0, 6, 0);
        lblLang.Name = "lblLang";
        lblLang.Size = new Size(205, 32);
        lblLang.TabIndex = 16;
        lblLang.Text = "Primary language:";
        // 
        // IDC_COMBO_LANG
        // 
        IDC_COMBO_LANG.Location = new Point(247, 196);
        IDC_COMBO_LANG.Margin = new Padding(6);
        IDC_COMBO_LANG.Name = "IDC_COMBO_LANG";
        IDC_COMBO_LANG.Size = new Size(261, 40);
        IDC_COMBO_LANG.TabIndex = 16;
        // 
        // lblPriority
        // 
        lblPriority.AutoSize = true;
        lblPriority.Location = new Point(28, 277);
        lblPriority.Margin = new Padding(6, 0, 6, 0);
        lblPriority.Name = "lblPriority";
        lblPriority.Size = new Size(140, 32);
        lblPriority.TabIndex = 17;
        lblPriority.Text = "Rating rank:";
        // 
        // IDC_SPIN_PRIORITY
        // 
        IDC_SPIN_PRIORITY.Location = new Point(247, 271);
        IDC_SPIN_PRIORITY.Margin = new Padding(6);
        IDC_SPIN_PRIORITY.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
        IDC_SPIN_PRIORITY.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        IDC_SPIN_PRIORITY.Name = "IDC_SPIN_PRIORITY";
        IDC_SPIN_PRIORITY.Size = new Size(149, 39);
        IDC_SPIN_PRIORITY.TabIndex = 17;
        IDC_SPIN_PRIORITY.Value = new decimal(new int[] { 1, 0, 0, 0 });
        // 
        // lblItems
        // 
        lblItems.AutoSize = true;
        lblItems.Location = new Point(520, 53);
        lblItems.Margin = new Padding(6, 0, 6, 0);
        lblItems.Name = "lblItems";
        lblItems.Size = new Size(147, 32);
        lblItems.TabIndex = 18;
        lblItems.Text = "Department:";
        // 
        // IDC_LIST_ITEMS
        // 
        IDC_LIST_ITEMS.Location = new Point(520, 102);
        IDC_LIST_ITEMS.Margin = new Padding(6);
        IDC_LIST_ITEMS.Name = "IDC_LIST_ITEMS";
        IDC_LIST_ITEMS.Size = new Size(359, 228);
        IDC_LIST_ITEMS.TabIndex = 20;
        // 
        // lblEmployment
        // 
        lblEmployment.AutoSize = true;
        lblEmployment.Location = new Point(28, 358);
        lblEmployment.Margin = new Padding(6, 0, 6, 0);
        lblEmployment.Name = "lblEmployment";
        lblEmployment.Size = new Size(154, 32);
        lblEmployment.TabIndex = 21;
        lblEmployment.Text = "Employment:";
        // 
        // pnlEmployment
        // 
        pnlEmployment.Controls.Add(IDC_RADIO_FULL_TIME);
        pnlEmployment.Controls.Add(IDC_RADIO_CONTRACT);
        pnlEmployment.Controls.Add(IDC_RADIO_PART_TIME);
        pnlEmployment.Location = new Point(247, 348);
        pnlEmployment.Margin = new Padding(6);
        pnlEmployment.Name = "pnlEmployment";
        pnlEmployment.Size = new Size(594, 58);
        pnlEmployment.TabIndex = 22;
        // 
        // IDC_RADIO_FULL_TIME
        // 
        IDC_RADIO_FULL_TIME.AutoSize = true;
        IDC_RADIO_FULL_TIME.Checked = true;
        IDC_RADIO_FULL_TIME.Location = new Point(10, 6);
        IDC_RADIO_FULL_TIME.Margin = new Padding(6);
        IDC_RADIO_FULL_TIME.Name = "IDC_RADIO_FULL_TIME";
        IDC_RADIO_FULL_TIME.Size = new Size(140, 36);
        IDC_RADIO_FULL_TIME.TabIndex = 23;
        IDC_RADIO_FULL_TIME.TabStop = true;
        IDC_RADIO_FULL_TIME.Text = "Full-time";
        // 
        // IDC_RADIO_CONTRACT
        // 
        IDC_RADIO_CONTRACT.AutoSize = true;
        IDC_RADIO_CONTRACT.Location = new Point(155, 6);
        IDC_RADIO_CONTRACT.Margin = new Padding(6);
        IDC_RADIO_CONTRACT.Name = "IDC_RADIO_CONTRACT";
        IDC_RADIO_CONTRACT.Size = new Size(135, 36);
        IDC_RADIO_CONTRACT.TabIndex = 24;
        IDC_RADIO_CONTRACT.Text = "Contract";
        // 
        // IDC_RADIO_PART_TIME
        // 
        IDC_RADIO_PART_TIME.AutoSize = true;
        IDC_RADIO_PART_TIME.Location = new Point(310, 6);
        IDC_RADIO_PART_TIME.Margin = new Padding(6);
        IDC_RADIO_PART_TIME.Name = "IDC_RADIO_PART_TIME";
        IDC_RADIO_PART_TIME.Size = new Size(144, 36);
        IDC_RADIO_PART_TIME.TabIndex = 25;
        IDC_RADIO_PART_TIME.Text = "Part-time";
        // 
        // grpProjects
        // 
        grpProjects.Controls.Add(IDC_GRID_ITEMS);
        grpProjects.Location = new Point(19, 1035);
        grpProjects.Margin = new Padding(6);
        grpProjects.Name = "grpProjects";
        grpProjects.Padding = new Padding(6);
        grpProjects.Size = new Size(910, 416);
        grpProjects.TabIndex = 3;
        grpProjects.TabStop = false;
        grpProjects.Text = "Project history";
        // 
        // IDC_GRID_ITEMS
        // 
        IDC_GRID_ITEMS.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        IDC_GRID_ITEMS.ColumnHeadersHeight = 46;
        IDC_GRID_ITEMS.Location = new Point(19, 47);
        IDC_GRID_ITEMS.Margin = new Padding(6);
        IDC_GRID_ITEMS.Name = "IDC_GRID_ITEMS";
        IDC_GRID_ITEMS.RowHeadersWidth = 82;
        IDC_GRID_ITEMS.Size = new Size(864, 341);
        IDC_GRID_ITEMS.TabIndex = 22;
        // 
        // IDC_STATIC_STATUS
        // 
        IDC_STATIC_STATUS.BorderStyle = BorderStyle.Fixed3D;
        IDC_STATIC_STATUS.Location = new Point(19, 1472);
        IDC_STATIC_STATUS.Margin = new Padding(6, 0, 6, 0);
        IDC_STATIC_STATUS.Name = "IDC_STATIC_STATUS";
        IDC_STATIC_STATUS.Size = new Size(910, 43);
        IDC_STATIC_STATUS.TabIndex = 21;
        IDC_STATIC_STATUS.Text = "Ready";
        // 
        // btnOk
        // 
        btnOk.Location = new Point(19, 1536);
        btnOk.Margin = new Padding(6);
        btnOk.Name = "btnOk";
        btnOk.Size = new Size(167, 64);
        btnOk.TabIndex = 30;
        btnOk.Text = "Save";
        btnOk.Click += btnOk_Click;
        // 
        // btnCancel
        // 
        btnCancel.Location = new Point(204, 1536);
        btnCancel.Margin = new Padding(6);
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(167, 64);
        btnCancel.TabIndex = 31;
        btnCancel.Text = "Reset";
        btnCancel.Click += btnCancel_Click;
        // 
        // btnNew
        // 
        btnNew.Location = new Point(390, 1536);
        btnNew.Margin = new Padding(6);
        btnNew.Name = "btnNew";
        btnNew.Size = new Size(167, 64);
        btnNew.TabIndex = 32;
        btnNew.Text = "New";
        btnNew.Click += btnNew_Click;
        // 
        // grpDebug
        // 
        grpDebug.Controls.Add(grpDdxState);
        grpDebug.Controls.Add(grpOpLog);
        grpDebug.Location = new Point(956, 64);
        grpDebug.Margin = new Padding(6);
        grpDebug.Name = "grpDebug";
        grpDebug.Padding = new Padding(6);
        grpDebug.Size = new Size(864, 1515);
        grpDebug.TabIndex = 33;
        grpDebug.TabStop = false;
        grpDebug.Text = "Debug panel";
        // 
        // grpDdxState
        // 
        grpDdxState.Controls.Add(dgvDdxState);
        grpDdxState.Location = new Point(15, 43);
        grpDdxState.Margin = new Padding(6);
        grpDdxState.Name = "grpDdxState";
        grpDdxState.Padding = new Padding(6);
        grpDdxState.Size = new Size(834, 672);
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
        dgvDdxState.Location = new Point(15, 47);
        dgvDdxState.Margin = new Padding(6);
        dgvDdxState.Name = "dgvDdxState";
        dgvDdxState.ReadOnly = true;
        dgvDdxState.RowHeadersVisible = false;
        dgvDdxState.RowHeadersWidth = 82;
        dgvDdxState.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvDdxState.Size = new Size(804, 597);
        dgvDdxState.TabIndex = 0;
        dgvDdxState.TabStop = false;
        // 
        // grpOpLog
        // 
        grpOpLog.Controls.Add(rtbOpLog);
        grpOpLog.Controls.Add(btnClearLog);
        grpOpLog.Controls.Add(btnRefreshState);
        grpOpLog.Controls.Add(btnRunUpdateFalse);
        grpOpLog.Controls.Add(btnRunUpdateTrue);
        grpOpLog.Location = new Point(15, 736);
        grpOpLog.Margin = new Padding(6);
        grpOpLog.Name = "grpOpLog";
        grpOpLog.Padding = new Padding(6);
        grpOpLog.Size = new Size(834, 757);
        grpOpLog.TabIndex = 1;
        grpOpLog.TabStop = false;
        grpOpLog.Text = "Operation log (DDX/DDV)";
        // 
        // rtbOpLog
        // 
        rtbOpLog.BackColor = Color.FromArgb(30, 30, 30);
        rtbOpLog.Font = new Font("Consolas", 8.25F);
        rtbOpLog.ForeColor = Color.LightGray;
        rtbOpLog.Location = new Point(15, 47);
        rtbOpLog.Margin = new Padding(6);
        rtbOpLog.Name = "rtbOpLog";
        rtbOpLog.ReadOnly = true;
        rtbOpLog.ScrollBars = RichTextBoxScrollBars.Vertical;
        rtbOpLog.Size = new Size(801, 621);
        rtbOpLog.TabIndex = 0;
        rtbOpLog.TabStop = false;
        rtbOpLog.Text = "";
        rtbOpLog.WordWrap = false;
        // 
        // btnClearLog
        // 
        btnClearLog.Location = new Point(15, 687);
        btnClearLog.Margin = new Padding(6);
        btnClearLog.Name = "btnClearLog";
        btnClearLog.Size = new Size(149, 53);
        btnClearLog.TabIndex = 50;
        btnClearLog.Text = "Clear log";
        btnClearLog.Click += btnClearLog_Click;
        // 
        // btnRefreshState
        // 
        btnRefreshState.Location = new Point(182, 687);
        btnRefreshState.Margin = new Padding(6);
        btnRefreshState.Name = "btnRefreshState";
        btnRefreshState.Size = new Size(149, 53);
        btnRefreshState.TabIndex = 51;
        btnRefreshState.Text = "Refresh state";
        btnRefreshState.Click += btnRefreshState_Click;
        // 
        // btnRunUpdateFalse
        // 
        btnRunUpdateFalse.Location = new Point(349, 687);
        btnRunUpdateFalse.Margin = new Padding(6);
        btnRunUpdateFalse.Name = "btnRunUpdateFalse";
        btnRunUpdateFalse.Size = new Size(158, 53);
        btnRunUpdateFalse.TabIndex = 52;
        btnRunUpdateFalse.Text = "▶ Doc→UI";
        btnRunUpdateFalse.Click += btnRunUpdateFalse_Click;
        // 
        // btnRunUpdateTrue
        // 
        btnRunUpdateTrue.Location = new Point(526, 687);
        btnRunUpdateTrue.Margin = new Padding(6);
        btnRunUpdateTrue.Name = "btnRunUpdateTrue";
        btnRunUpdateTrue.Size = new Size(204, 53);
        btnRunUpdateTrue.TabIndex = 53;
        btnRunUpdateTrue.Text = "▶ UI→Doc+DDV";
        btnRunUpdateTrue.Click += btnRunUpdateTrue_Click;
        // 
        // SampleView
        // 
        AutoScaleDimensions = new SizeF(13F, 32F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1839, 1621);
        Controls.Add(menuStrip1);
        Controls.Add(grpBasicInfo);
        Controls.Add(grpWorkInfo);
        Controls.Add(grpProjects);
        Controls.Add(IDC_STATIC_STATUS);
        Controls.Add(btnOk);
        Controls.Add(btnCancel);
        Controls.Add(btnNew);
        Controls.Add(grpDebug);
        MainMenuStrip = menuStrip1;
        Margin = new Padding(6);
        Name = "SampleView";
        Text = "Employee information";
        menuStrip1.ResumeLayout(false);
        menuStrip1.PerformLayout();
        grpBasicInfo.ResumeLayout(false);
        grpBasicInfo.PerformLayout();
        pnlGender.ResumeLayout(false);
        pnlGender.PerformLayout();
        grpWorkInfo.ResumeLayout(false);
        grpWorkInfo.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)IDC_SPIN_PRIORITY).EndInit();
        pnlEmployment.ResumeLayout(false);
        pnlEmployment.PerformLayout();
        grpProjects.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)IDC_GRID_ITEMS).EndInit();
        grpDebug.ResumeLayout(false);
        grpDdxState.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)dgvDdxState).EndInit();
        grpOpLog.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

    private CheckBox checkBox1;
}
