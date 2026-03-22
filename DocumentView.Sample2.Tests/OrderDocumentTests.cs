using System.ComponentModel;
using DocumentView.Framework;
using DocumentView.Sample2;
using Xunit;

namespace DocumentView.Sample2.Tests;

// ══════════════════════════════════════════════════════════════════════════════
// SupplierDocument Tests  (IDD_SUPPLIER_INFO 相当)
// ══════════════════════════════════════════════════════════════════════════════
public class SupplierDocumentTests
{
    private static SupplierDocument NewDoc(FakeMessageBoxService? fake = null)
        => new(fake ?? new FakeMessageBoxService());

    // ── Initial values ──────────────────────────────────────────────────────

    [Fact]
    public void InitialValues_AreCorrect()
    {
        var doc = NewDoc();
        Assert.Equal(string.Empty, doc.m_strSupplierCode);
        Assert.Equal(string.Empty, doc.m_strSupplierName);
        Assert.Equal(string.Empty, doc.m_strAddress);
        Assert.Equal(string.Empty, doc.m_strTel);
        Assert.Equal(string.Empty, doc.m_strFax);
    }

    // ── Reset ───────────────────────────────────────────────────────────────

    [Fact]
    public void Reset_ClearsAllFields()
    {
        var doc = NewDoc();
        doc.m_strSupplierCode = "SUP001";
        doc.m_strSupplierName = "テスト仕入先";

        doc.Reset();

        Assert.Equal(string.Empty, doc.m_strSupplierCode);
        Assert.Equal(string.Empty, doc.m_strSupplierName);
    }

    // ── ValidateBusinessRule ─────────────────────────────────────────────────

    [Fact]
    public void ValidateBusinessRule_EmptySupplierCode_ReturnsFalse()
    {
        var doc = NewDoc();
        doc.m_strSupplierCode = "";

        Assert.False(doc.ValidateBusinessRule(out var error));
        Assert.False(string.IsNullOrEmpty(error));
    }

    [Fact]
    public void ValidateBusinessRule_WhitespaceSupplierCode_ReturnsFalse()
    {
        var doc = NewDoc();
        doc.m_strSupplierCode = "   ";

        Assert.False(doc.ValidateBusinessRule(out _));
    }

    [Fact]
    public void ValidateBusinessRule_ValidCode_ReturnsTrue()
    {
        var doc = NewDoc();
        doc.m_strSupplierCode = "SUP001";

        Assert.True(doc.ValidateBusinessRule(out var error));
        Assert.Equal(string.Empty, error);
    }

    // ── DDX mapping count ────────────────────────────────────────────────────

    [StaFact]
    public void GetDdxMappings_AfterAttachView_Returns5Fields()
    {
        var doc = NewDoc();
        using var form = CreateStub();
        doc.AttachView(form);

        Assert.Equal(5, doc.GetDdxMappings().Count);
    }

    // ── DDV: MaxChars ────────────────────────────────────────────────────────

    [StaFact]
    public void UpdateData_SupplierCodeExceedsMaxChars_ShowsWarning()
    {
        var fake = new FakeMessageBoxService();
        var doc  = NewDoc(fake);
        using var form = CreateStub();
        ((TextBox)form.Controls[OrderView.Ctrl.IDC_EDIT_SUPPLIER_CODE]!).Text = new string('X', 11);
        doc.AttachView(form);

        doc.UpdateData(true);

        Assert.Single(fake.Calls);
        Assert.Equal(MessageBoxIcon.Warning, fake.Calls[0].Icon);
    }

    // ── Helpers ─────────────────────────────────────────────────────────────
    private static Form CreateStub()
    {
        var form = new Form();
        form.Controls.Add(new TextBox { Name = OrderView.Ctrl.IDC_EDIT_SUPPLIER_CODE });
        form.Controls.Add(new TextBox { Name = OrderView.Ctrl.IDC_EDIT_SUPPLIER_NAME });
        form.Controls.Add(new TextBox { Name = OrderView.Ctrl.IDC_EDIT_ADDRESS });
        form.Controls.Add(new TextBox { Name = OrderView.Ctrl.IDC_EDIT_TEL });
        form.Controls.Add(new TextBox { Name = OrderView.Ctrl.IDC_EDIT_FAX });
        return form;
    }
}

// ══════════════════════════════════════════════════════════════════════════════
// OrderHeaderDocument Tests  (IDD_ORDER_HEADER 相当)
// ══════════════════════════════════════════════════════════════════════════════
public class OrderHeaderDocumentTests
{
    private static OrderHeaderDocument NewDoc(FakeMessageBoxService? fake = null)
        => new(fake ?? new FakeMessageBoxService());

    // ── Initial values ──────────────────────────────────────────────────────

    [Fact]
    public void InitialValues_AreCorrect()
    {
        var doc = NewDoc();
        Assert.Equal(string.Empty, doc.m_strOrderNo);
        Assert.Equal(string.Empty, doc.m_strOrderDate);
        Assert.Equal(string.Empty, doc.m_strDeliveryDate);
        Assert.Equal(0,            doc.m_nStatus);
        Assert.False(doc.m_bUrgent);
        Assert.Equal(string.Empty, doc.m_strMemo);
    }

    // ── Reset ───────────────────────────────────────────────────────────────

    [Fact]
    public void Reset_SetsOrderNoAndDate()
    {
        var doc = NewDoc();
        doc.Reset("PO-20260322_0001");

        Assert.Equal("PO-20260322_0001",              doc.m_strOrderNo);
        Assert.Equal(DateTime.Today.ToString("yyyy/MM/dd"), doc.m_strOrderDate);
        Assert.Equal(0,     doc.m_nStatus);
        Assert.False(doc.m_bUrgent);
    }

    // ── DDX mapping count ────────────────────────────────────────────────────

    [StaFact]
    public void GetDdxMappings_AfterAttachView_Returns6Fields()
    {
        var doc = NewDoc();
        using var form = CreateStub();
        doc.AttachView(form);

        Assert.Equal(6, doc.GetDdxMappings().Count);
    }

    // ── OnStatusChanged ──────────────────────────────────────────────────────

    [StaFact]
    public void OnStatusChanged_Status3_DisablesAllControls()
    {
        var doc = NewDoc();
        using var form = CreateFullStub();
        doc.AttachView(form);
        doc.AttachResourceId(typeof(ResourceId));

        ((ComboBox)form.Controls[OrderView.Ctrl.IDC_COMBO_STATUS]!).SelectedIndex = 3;
        doc.OnStatusChanged();

        Assert.False(form.Controls[OrderView.Ctrl.IDC_EDIT_SUPPLIER_CODE]!.Enabled);
        Assert.False(form.Controls[OrderView.Ctrl.IDC_EDIT_ORDER_DATE]!.Enabled);
        Assert.False(form.Controls[OrderView.Ctrl.IDC_COMBO_STATUS]!.Enabled);
        Assert.False(form.Controls[OrderView.Ctrl.IDC_GRID_LINES]!.Enabled);
    }

    [StaFact]
    public void OnStatusChanged_StatusNot3_EnablesAllControls()
    {
        var doc = NewDoc();
        using var form = CreateFullStub();
        doc.AttachView(form);
        doc.AttachResourceId(typeof(ResourceId));

        // 一旦ロック
        ((ComboBox)form.Controls[OrderView.Ctrl.IDC_COMBO_STATUS]!).SelectedIndex = 3;
        doc.OnStatusChanged();
        // アンロック
        ((ComboBox)form.Controls[OrderView.Ctrl.IDC_COMBO_STATUS]!).SelectedIndex = 1;
        doc.OnStatusChanged();

        Assert.True(form.Controls[OrderView.Ctrl.IDC_EDIT_SUPPLIER_CODE]!.Enabled);
        Assert.True(form.Controls[OrderView.Ctrl.IDC_EDIT_ORDER_DATE]!.Enabled);
        Assert.True(form.Controls[OrderView.Ctrl.IDC_COMBO_STATUS]!.Enabled);
        Assert.True(form.Controls[OrderView.Ctrl.IDC_GRID_LINES]!.Enabled);
    }

    // ── Helpers ─────────────────────────────────────────────────────────────
    private static Form CreateStub()
    {
        var form = new Form();
        form.Controls.Add(new TextBox  { Name = OrderView.Ctrl.IDC_EDIT_ORDER_NO });
        form.Controls.Add(new TextBox  { Name = OrderView.Ctrl.IDC_EDIT_ORDER_DATE });
        form.Controls.Add(new TextBox  { Name = OrderView.Ctrl.IDC_EDIT_DELIVERY_DATE });
        var cmb = new ComboBox { Name = OrderView.Ctrl.IDC_COMBO_STATUS };
        cmb.Items.AddRange(["未発注", "発注済", "納品待ち", "入荷済み"]);
        form.Controls.Add(cmb);
        form.Controls.Add(new CheckBox { Name = OrderView.Ctrl.IDC_CHECK_URGENT });
        form.Controls.Add(new TextBox  { Name = OrderView.Ctrl.IDC_EDIT_MEMO, Multiline = true });
        return form;
    }

    /// <summary>OnStatusChanged の SetEnabled テスト用に全IDC_*コントロールを含む。</summary>
    private static Form CreateFullStub()
    {
        var form = CreateStub();
        form.Controls.Add(new TextBox     { Name = OrderView.Ctrl.IDC_EDIT_SUPPLIER_CODE });
        form.Controls.Add(new TextBox     { Name = OrderView.Ctrl.IDC_EDIT_SUPPLIER_NAME });
        form.Controls.Add(new TextBox     { Name = OrderView.Ctrl.IDC_EDIT_ADDRESS });
        form.Controls.Add(new TextBox     { Name = OrderView.Ctrl.IDC_EDIT_TEL });
        form.Controls.Add(new TextBox     { Name = OrderView.Ctrl.IDC_EDIT_FAX });
        form.Controls.Add(new DataGridView { Name = OrderView.Ctrl.IDC_GRID_LINES });
        form.Controls.Add(new Label       { Name = OrderView.Ctrl.IDC_LABEL_TOTAL });
        return form;
    }
}

// ══════════════════════════════════════════════════════════════════════════════
// OrderDetailDocument Tests  (IDD_ORDER_DETAIL 相当)
// ══════════════════════════════════════════════════════════════════════════════
public class OrderDetailDocumentTests
{
    private static OrderDetailDocument NewDoc(FakeMessageBoxService? fake = null)
        => new(fake ?? new FakeMessageBoxService());

    // ── Initial values ──────────────────────────────────────────────────────

    [Fact]
    public void InitialValues_AreCorrect()
    {
        var doc = NewDoc();
        Assert.Empty(doc.m_gridLines);
        Assert.Equal("¥0", doc.m_strTotal);
    }

    // ── Reset ───────────────────────────────────────────────────────────────

    [Fact]
    public void Reset_ClearsGridAndTotal()
    {
        var doc = NewDoc();
        doc.m_gridLines.Add(new OrderLine { Qty = 1, UnitPrice = 500m });
        doc.m_strTotal = "¥500";

        doc.Reset();

        Assert.Empty(doc.m_gridLines);
        Assert.Equal("¥0", doc.m_strTotal);
    }

    // ── RecalculateTotal ─────────────────────────────────────────────────────

    [Fact]
    public void RecalculateTotal_EmptyGrid_SetsZero()
    {
        var doc = NewDoc();
        doc.RecalculateTotal();
        Assert.Equal("¥0", doc.m_strTotal);
    }

    [Fact]
    public void RecalculateTotal_WithLines_SumsCorrectly()
    {
        var doc = NewDoc();
        doc.m_gridLines = new BindingList<OrderLine>
        {
            new() { Qty = 2, UnitPrice = 1000m },
            new() { Qty = 3, UnitPrice = 500m  },
        };

        doc.RecalculateTotal();

        Assert.Equal("¥3,500", doc.m_strTotal);
    }

    // ── SubscribeGridLines ───────────────────────────────────────────────────

    [Fact]
    public void SubscribeGridLines_RecalculatesOnAdd()
    {
        var doc = NewDoc();
        doc.m_gridLines = new BindingList<OrderLine>();
        doc.SubscribeGridLines();

        doc.m_gridLines.Add(new OrderLine { Qty = 1, UnitPrice = 2000m });

        Assert.Equal("¥2,000", doc.m_strTotal);
    }

    [Fact]
    public void SubscribeGridLines_RecalculatesOnPropertyChange()
    {
        var line = new OrderLine { Qty = 1, UnitPrice = 1000m };
        var doc  = NewDoc();
        doc.m_gridLines = new BindingList<OrderLine> { line };
        doc.SubscribeGridLines();

        line.Qty = 5;

        Assert.Equal("¥5,000", doc.m_strTotal);
    }

    // ── DDX mapping count ────────────────────────────────────────────────────

    [StaFact]
    public void GetDdxMappings_AfterAttachView_Returns2Fields()
    {
        var doc = NewDoc();
        using var form = new Form();
        form.Controls.Add(new DataGridView { Name = OrderView.Ctrl.IDC_GRID_LINES });
        form.Controls.Add(new Label        { Name = OrderView.Ctrl.IDC_LABEL_TOTAL });
        doc.AttachView(form);

        Assert.Equal(2, doc.GetDdxMappings().Count);
    }
}

// ══════════════════════════════════════════════════════════════════════════════
// OrderView Integration Tests  (保存/読み込みのラウンドトリップ)
// ══════════════════════════════════════════════════════════════════════════════
public class OrderViewIntegrationTests : IDisposable
{
    private readonly string _testDataDir =
        Path.Combine(Path.GetTempPath(), "OrderViewTests_" + Guid.NewGuid().ToString("N"));

    public OrderViewIntegrationTests()
    {
        OrderView.DataDirectory = _testDataDir;
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDataDir))
            Directory.Delete(_testDataDir, recursive: true);
    }

    private static (SupplierDocument s, OrderHeaderDocument h, OrderDetailDocument d, FakeMessageBoxService fake)
        NewDocs()
    {
        var fake = new FakeMessageBoxService();
        return (new SupplierDocument(fake), new OrderHeaderDocument(fake), new OrderDetailDocument(fake), fake);
    }

    [StaFact]
    public void OnBtnSave_WithEmptySupplierCode_ShowsWarning()
    {
        var (s, h, d, fake) = NewDocs();
        using var view = new OrderView(s, h, d, fake);
        view.OnNew();

        // 仕入先コードを空のままで保存
        s.m_strSupplierCode = "";
        view.OnBtnSave();

        Assert.Single(fake.Calls);
        Assert.Equal(MessageBoxIcon.Warning, fake.Calls[0].Icon);
    }

    [StaFact]
    public void OnBtnSave_WithValidData_SavesJsonFile()
    {
        var (s, h, d, fake) = NewDocs();
        using var view = new OrderView(s, h, d, fake);
        view.OnNew();

        s.m_strSupplierCode = "SUP001";
        s.UpdateData(false); // UIに反映してからOnBtnSave（UpdateData(true)がUIから読む）
        view.OnBtnSave();

        Assert.Empty(fake.Calls);
        Assert.True(Directory.GetFiles(_testDataDir, "PO-*.json").Length > 0);
    }

    [StaFact]
    public void OnMenuLoad_WithNonExistentOrderNo_ShowsWarning()
    {
        var (s, h, d, fake) = NewDocs();
        using var view = new OrderView(s, h, d, fake);
        view.OnNew();

        h.m_strOrderNo = "PO-99990101_9999";
        view.OnMenuLoad();

        Assert.Single(fake.Calls);
        Assert.Equal(MessageBoxIcon.Warning, fake.Calls[0].Icon);
    }

    [StaFact]
    public void OnMenuLoad_RoundTrip_RestoresAllDocuments()
    {
        // 保存
        var (s1, h1, d1, fake1) = NewDocs();
        using var saveView = new OrderView(s1, h1, d1, fake1);
        saveView.OnNew();
        s1.m_strSupplierCode = "SUP999";
        s1.m_strSupplierName = "テスト株式会社";
        h1.m_bUrgent         = true;
        d1.m_gridLines.Add(new OrderLine { ItemCode = "A-001", ItemName = "テスト品", Qty = 3, UnitPrice = 1500m });
        // UIに反映してからOnBtnSave（UpdateData(true)がUIから読む）
        s1.UpdateData(false);
        h1.UpdateData(false);
        var savedOrderNo = h1.m_strOrderNo;
        saveView.OnBtnSave();

        // 読み込み
        var (s2, h2, d2, fake2) = NewDocs();
        using var loadView = new OrderView(s2, h2, d2, fake2);
        loadView.OnNew();
        h2.m_strOrderNo = savedOrderNo;
        loadView.OnMenuLoad();

        Assert.Equal("SUP999",         s2.m_strSupplierCode);
        Assert.Equal("テスト株式会社",  s2.m_strSupplierName);
        Assert.True(h2.m_bUrgent);
        Assert.Single(d2.m_gridLines);
        Assert.Equal("A-001",  d2.m_gridLines[0].ItemCode);
        Assert.Equal(3,        d2.m_gridLines[0].Qty);
        Assert.Equal(1500m,    d2.m_gridLines[0].UnitPrice);
    }
}
