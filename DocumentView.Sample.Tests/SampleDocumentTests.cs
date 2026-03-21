using DocumentView.Framework;
using DocumentView.Sample;
using Xunit;

namespace DocumentView.Sample.Tests;

public class SampleDocumentTests : IDisposable
{
    // Per-test isolated temp directory
    private readonly string _testDataDir =
        Path.Combine(Path.GetTempPath(), "SampleDocumentTests_" + Guid.NewGuid().ToString("N"));

    public SampleDocumentTests()
    {
        SampleDocument.DataDirectory = _testDataDir;
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDataDir))
            Directory.Delete(_testDataDir, recursive: true);
    }

    // ── Helpers ────────────────────────────────────────────────────────────

    /// <summary>Test factory; always uses <see cref="FakeMessageBoxService"/> for message boxes.</summary>
    private static SampleDocument NewDoc(FakeMessageBoxService? fake = null)
        => new(fake ?? new FakeMessageBoxService());

    /// <summary>
    /// Builds a <see cref="Form"/> with the minimum controls for SampleDocument DDX.
    /// Control names match DDX without using SampleView.Designer.cs.
    /// </summary>
    private static Form CreateSampleViewStub()
    {
        var form = new Form();
        void Add(Control c) { form.Controls.Add(c); }

        Add(new TextBox        { Name = SampleView.Ctrl.IDC_EDIT_EMPNO    });
        Add(new TextBox        { Name = SampleView.Ctrl.IDC_EDIT_NAME     });
        Add(new TextBox        { Name = SampleView.Ctrl.IDC_EDIT_AGE      });
        Add(new TextBox        { Name = SampleView.Ctrl.IDC_EDIT_MEMO     });
        Add(new CheckBox       { Name = SampleView.Ctrl.IDC_CHECK_ACTIVE  });
        Add(new CheckBox       { Name = SampleView.Ctrl.IDC_CHECK_NOTIFY  });
        Add(new ComboBox       { Name = SampleView.Ctrl.IDC_COMBO_PREF    });
        Add(new ComboBox       { Name = SampleView.Ctrl.IDC_COMBO_LANG    });
        Add(new NumericUpDown  { Name = SampleView.Ctrl.IDC_SPIN_PRIORITY,
                                 Minimum = 1, Maximum = 10, Value = 1     });
        Add(new RadioButton    { Name = SampleView.Ctrl.IDC_RADIO_MALE    });
        Add(new RadioButton    { Name = SampleView.Ctrl.IDC_RADIO_FEMALE  });
        Add(new ListBox        { Name = SampleView.Ctrl.IDC_LIST_ITEMS    });
        Add(new Label          { Name = SampleView.Ctrl.IDC_STATIC_STATUS });
        Add(new DataGridView   { Name = SampleView.Ctrl.IDC_GRID_ITEMS    });
        return form;
    }

    // ── ValidateBusinessRule ─────────────────────────────────────────────────

    [Fact]
    public void ValidateBusinessRule_EmptyEmpNo_ReturnsFalse()
    {
        var doc = NewDoc();
        doc.m_strEmpNo = "";
        doc.m_strName  = "Tanaka";

        Assert.False(doc.ValidateBusinessRule(out var error));
        Assert.False(string.IsNullOrEmpty(error));
    }

    [Fact]
    public void ValidateBusinessRule_EmptyName_ReturnsFalse()
    {
        var doc = NewDoc();
        doc.m_strEmpNo = "EMP001";
        doc.m_strName  = "";

        Assert.False(doc.ValidateBusinessRule(out var error));
        Assert.False(string.IsNullOrEmpty(error));
    }

    [Fact]
    public void ValidateBusinessRule_WhitespaceName_ReturnsFalse()
    {
        var doc = NewDoc();
        doc.m_strEmpNo = "EMP001";
        doc.m_strName  = "   ";

        Assert.False(doc.ValidateBusinessRule(out _));
    }

    [Fact]
    public void ValidateBusinessRule_ValidInput_ReturnsTrue()
    {
        var doc = NewDoc();
        doc.m_strEmpNo = "EMP001";
        doc.m_strName  = "Tanaka";

        Assert.True(doc.ValidateBusinessRule(out var error));
        Assert.Equal(string.Empty, error);
    }

    // ── Initial values ───────────────────────────────────────────────────────

    [Fact]
    public void InitialValues_AreCorrect()
    {
        var doc = NewDoc();

        Assert.Equal(string.Empty, doc.m_strEmpNo);
        Assert.Equal(string.Empty, doc.m_strName);
        Assert.Equal(0, doc.m_nAge);
        Assert.Equal(string.Empty, doc.m_strMemo);
        Assert.False(doc.m_bActive);
        Assert.False(doc.m_bNotify);
        Assert.Equal(0, doc.m_nPrefIndex);
        Assert.Equal(string.Empty, doc.m_strLang);
        Assert.Equal(1, doc.m_nPriority);
        Assert.True(doc.m_bMale);
        Assert.False(doc.m_bFemale);
        Assert.Equal(-1, doc.m_nItemIndex);
        Assert.Equal("Ready", doc.m_strStatus);
    }

    [Fact]
    public void GridItems_InitiallyHasThreeRows()
    {
        Assert.Equal(3, NewDoc().m_gridItems.Count);
    }

    [Fact]
    public void GridItems_InitialRows_HaveExpectedValues()
    {
        var doc = NewDoc();

        Assert.Equal(1, doc.m_gridItems[0].No);
        Assert.Equal("Core system modernization", doc.m_gridItems[0].Name);
        Assert.True(doc.m_gridItems[0].Active);

        Assert.Equal(2, doc.m_gridItems[1].No);
        Assert.Equal("Mobile app development", doc.m_gridItems[1].Name);
        Assert.True(doc.m_gridItems[1].Active);
    }

    // ── GetDdxMappings ───────────────────────────────────────────────────────

    [StaFact]
    public void GetDdxMappings_AfterAttachView_ReturnsAllDdxFields()
    {
        var doc = NewDoc();
        using var form = CreateSampleViewStub();
        doc.AttachView(form);

        var mappings = doc.GetDdxMappings();

        // SampleDocument has 17 DDX fields (after employment radio buttons)
        Assert.Equal(17, mappings.Count);
    }

    // ── IMessageBoxService injection tests ───────────────────────────────────

    [StaFact]
    public void OnBtnOk_WithEmptyName_ShowsErrorMessage()
    {
        var fake = new FakeMessageBoxService();
        var doc  = NewDoc(fake);
        using var form = CreateSampleViewStub();
        ((TextBox)form.Controls[SampleView.Ctrl.IDC_EDIT_EMPNO]!).Text = "EMP001";
        ((TextBox)form.Controls[SampleView.Ctrl.IDC_EDIT_NAME]!).Text  = "";
        doc.AttachView(form);

        doc.OnBtnOk();

        Assert.Single(fake.Calls);
        Assert.Equal(MessageBoxIcon.Warning, fake.Calls[0].Icon);
    }

    [StaFact]
    public void OnBtnOk_WithDDVViolation_ShowsErrorMessage()
    {
        var fake = new FakeMessageBoxService();
        var doc  = NewDoc(fake);
        using var form = CreateSampleViewStub();
        // 31 chars violates DDVMaxChars(30)
        ((TextBox)form.Controls[SampleView.Ctrl.IDC_EDIT_NAME]!).Text = new string('x', 31);
        doc.AttachView(form);

        doc.OnBtnOk();

        // DDV failure → MfcDocument shows message; OnBtnOk returns early
        Assert.Single(fake.Calls);
        Assert.Equal(MessageBoxIcon.Warning, fake.Calls[0].Icon);
    }

    [StaFact]
    public void OnBtnOk_WithValidInput_SavesFileAndDoesNotShowMessage()
    {
        var fake = new FakeMessageBoxService();
        var doc  = NewDoc(fake);
        using var form = CreateSampleViewStub();
        ((TextBox)form.Controls[SampleView.Ctrl.IDC_EDIT_EMPNO]!).Text = "EMP001";
        ((TextBox)form.Controls[SampleView.Ctrl.IDC_EDIT_NAME]!).Text  = "Tanaka";
        doc.AttachView(form);

        doc.OnBtnOk();

        Assert.Empty(fake.Calls);
        // File was written
        Assert.True(File.Exists(Path.Combine(_testDataDir, "EMP001.json")));
    }

    // ── OnMenuLoad ───────────────────────────────────────────────────────────

    [StaFact]
    public void OnMenuLoad_WithEmptyEmpNo_ShowsWarning()
    {
        var fake = new FakeMessageBoxService();
        var doc  = NewDoc(fake);
        using var form = CreateSampleViewStub();
        doc.AttachView(form);
        doc.AttachResourceId(typeof(ResourceId));
        // IDC_EDIT_EMPNO stays empty

        doc.OnMenuLoad();

        Assert.Single(fake.Calls);
        Assert.Equal(MessageBoxIcon.Warning, fake.Calls[0].Icon);
    }

    [StaFact]
    public void OnMenuLoad_WithNonExistentFile_ShowsWarning()
    {
        var fake = new FakeMessageBoxService();
        var doc  = NewDoc(fake);
        using var form = CreateSampleViewStub();
        ((TextBox)form.Controls[SampleView.Ctrl.IDC_EDIT_EMPNO]!).Text = "NOTEXIST";
        doc.AttachView(form);
        doc.AttachResourceId(typeof(ResourceId));

        doc.OnMenuLoad();

        Assert.Single(fake.Calls);
        Assert.Equal(MessageBoxIcon.Warning, fake.Calls[0].Icon);
    }

    [StaFact]
    public void OnMenuLoad_WithExistingFile_LoadsData()
    {
        // Save first
        var saveDoc = NewDoc();
        using var saveForm = CreateSampleViewStub();
        ((TextBox)saveForm.Controls[SampleView.Ctrl.IDC_EDIT_EMPNO]!).Text = "EMP999";
        ((TextBox)saveForm.Controls[SampleView.Ctrl.IDC_EDIT_NAME]!).Text  = "Suzuki";
        saveDoc.AttachView(saveForm);
        saveDoc.OnBtnOk();

        // Load with a different document instance
        var loadDoc = NewDoc();
        using var loadForm = CreateSampleViewStub();
        ((TextBox)loadForm.Controls[SampleView.Ctrl.IDC_EDIT_EMPNO]!).Text = "EMP999";
        loadDoc.AttachView(loadForm);
        loadDoc.AttachResourceId(typeof(ResourceId));

        loadDoc.OnMenuLoad();

        Assert.Equal("EMP999", loadDoc.m_strEmpNo);
        Assert.Equal("Suzuki",   loadDoc.m_strName);
    }

    // ── OnBtnDebug ───────────────────────────────────────────────────────────

    [Fact]
    public void OnBtnDebug_AlwaysShowsMessage()
    {
        var fake = new FakeMessageBoxService();
        var doc  = NewDoc(fake);

        doc.OnBtnDebug();

        Assert.Single(fake.Calls);
    }

    [Fact]
    public void OnBtnDebug_MessageText_ContainsDdxLabel()
    {
        var fake = new FakeMessageBoxService();
        var doc  = NewDoc(fake);

        doc.OnBtnDebug();

        Assert.Contains("DDX", fake.Calls[0].Text);
    }

    // ── OnCheckActiveChanged ─────────────────────────────────────────────────

    [StaFact]
    public void OnCheckActiveChanged_WhenChecked_EnablesControls()
    {
        var doc = NewDoc();
        using var form = CreateSampleViewStub();
        doc.AttachView(form);
        doc.AttachResourceId(typeof(ResourceId));

        ((CheckBox)form.Controls[SampleView.Ctrl.IDC_CHECK_ACTIVE]!).Checked = true;
        doc.OnCheckActiveChanged();

        Assert.True(form.Controls[SampleView.Ctrl.IDC_EDIT_NAME]!.Enabled);
        Assert.True(form.Controls[SampleView.Ctrl.IDC_EDIT_AGE]!.Enabled);
        Assert.True(form.Controls[SampleView.Ctrl.IDC_EDIT_MEMO]!.Enabled);
        Assert.True(form.Controls[SampleView.Ctrl.IDC_COMBO_PREF]!.Enabled);
        Assert.True(form.Controls[SampleView.Ctrl.IDC_COMBO_LANG]!.Enabled);
        Assert.True(form.Controls[SampleView.Ctrl.IDC_SPIN_PRIORITY]!.Enabled);
    }

    [StaFact]
    public void OnCheckActiveChanged_WhenUnchecked_DisablesControls()
    {
        var doc = NewDoc();
        using var form = CreateSampleViewStub();
        doc.AttachView(form);
        doc.AttachResourceId(typeof(ResourceId));

        ((CheckBox)form.Controls[SampleView.Ctrl.IDC_CHECK_ACTIVE]!).Checked = false;
        doc.OnCheckActiveChanged();

        Assert.False(form.Controls[SampleView.Ctrl.IDC_EDIT_NAME]!.Enabled);
        Assert.False(form.Controls[SampleView.Ctrl.IDC_EDIT_AGE]!.Enabled);
        Assert.False(form.Controls[SampleView.Ctrl.IDC_EDIT_MEMO]!.Enabled);
        Assert.False(form.Controls[SampleView.Ctrl.IDC_COMBO_PREF]!.Enabled);
        Assert.False(form.Controls[SampleView.Ctrl.IDC_COMBO_LANG]!.Enabled);
        Assert.False(form.Controls[SampleView.Ctrl.IDC_SPIN_PRIORITY]!.Enabled);
    }

    // ── OnBtnShowGrid ────────────────────────────────────────────────────────

    [Fact]
    public void OnBtnShowGrid_AlwaysShowsMessage()
    {
        var fake = new FakeMessageBoxService();
        var doc  = NewDoc(fake);

        doc.OnBtnShowGrid();

        Assert.Single(fake.Calls);
    }

    [Fact]
    public void OnBtnShowGrid_Caption_IsGridDataTitle()
    {
        var fake = new FakeMessageBoxService();
        var doc  = NewDoc(fake);

        doc.OnBtnShowGrid();

        Assert.Equal("Grid data", fake.Calls[0].Caption);
    }

    [Fact]
    public void OnBtnShowGrid_MessageText_ContainsGridItems()
    {
        var fake = new FakeMessageBoxService();
        var doc  = NewDoc(fake);

        doc.OnBtnShowGrid();

        Assert.Contains("Core system modernization", fake.Calls[0].Text);
        Assert.Contains("Mobile app development", fake.Calls[0].Text);
        Assert.Contains("AI adoption support", fake.Calls[0].Text);
    }
}
