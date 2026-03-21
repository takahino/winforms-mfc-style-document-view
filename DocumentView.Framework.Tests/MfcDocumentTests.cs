using DocumentView.Framework;
using Xunit;

namespace DocumentView.Framework.Tests;

public class MfcDocumentTests
{
    // ── Test documents ───────────────────────────────────────────────────────

    private class TestDoc : MfcDocument
    {
        public TestDoc(IMessageBoxService? svc = null) : base(svc ?? new FakeMessageBoxService()) { }

        [DDX("tb1")]
        public string StrField = "initial";

        [DDX("cb1")]
        public bool BoolField = false;

        [DDX("nud1")]
        [DDVMinMax(0, 100)]
        public int IntField = 50;
    }

    // Document for ResourceId-based control tests
    private class ResourceDoc : MfcDocument
    {
        public ResourceDoc() : base(new FakeMessageBoxService()) { }

        [DDX("tb1")]
        public string Name = "";

        private static class Ids
        {
            [AutoId("tb1")] public static int TB1;
            static Ids() => AutoIdAssigner.Assign(typeof(Ids));
        }

        public void AttachIds() => AttachResourceId(typeof(Ids));

        public Control? GetTb1() => GetControl(Ids.TB1);
        public void DisableTb1()  => SetEnabled(Ids.TB1, false);
        public void EnableTb1()   => SetEnabled(Ids.TB1, true);
        public void HideTb1()     => SetVisible(Ids.TB1, false);
        public void ShowTb1()     => SetVisible(Ids.TB1, true);
    }

    // ── Helpers ────────────────────────────────────────────────────────────

    private static Form CreateForm()
    {
        var form = new Form();
        form.Controls.Add(new TextBox        { Name = "tb1",  Text     = "fromUI" });
        form.Controls.Add(new CheckBox       { Name = "cb1",  Checked  = true     });
        form.Controls.Add(new NumericUpDown  { Name = "nud1", Minimum  = 0,
                                               Maximum = 100, Value    = 75       });
        return form;
    }

    // ── UpdateData(false): Document → UI ────────────────────────────────────

    [StaFact]
    public void UpdateData_False_TransfersDocumentFieldsToControls()
    {
        var doc = new TestDoc { StrField = "docValue", BoolField = true, IntField = 30 };
        using var form = CreateForm();
        doc.AttachView(form);

        doc.UpdateData(false);

        Assert.Equal("docValue", ((TextBox)form.Controls["tb1"]!).Text);
        Assert.True(((CheckBox)form.Controls["cb1"]!).Checked);
        Assert.Equal(30m, ((NumericUpDown)form.Controls["nud1"]!).Value);
    }

    // ── UpdateData(true): UI → Document ─────────────────────────────────────

    [StaFact]
    public void UpdateData_True_TransfersControlsToDocumentFields()
    {
        var doc = new TestDoc();
        using var form = CreateForm();
        ((TextBox)form.Controls["tb1"]!).Text     = "newValue";
        ((CheckBox)form.Controls["cb1"]!).Checked = true;
        doc.AttachView(form);

        var result = doc.UpdateData(true);

        Assert.True(result);
        Assert.Equal("newValue", doc.StrField);
        Assert.True(doc.BoolField);
        Assert.Equal(75, doc.IntField); // NumericUpDown initial value 75
    }

    [StaFact]
    public void UpdateData_True_ReturnsTrue_WhenDDVPasses()
    {
        var fake = new FakeMessageBoxService();
        var doc  = new TestDoc(fake);
        using var form = CreateForm(); // nud1.Value = 75, within [0, 100]
        doc.AttachView(form);

        var result = doc.UpdateData(true);

        Assert.True(result);
        Assert.Empty(fake.Calls); // no message box
    }

    [StaFact]
    public void UpdateData_True_ReturnsFalse_WhenDDVFails()
    {
        // 7 chars violates DDVMaxChars(5)
        var fake = new FakeMessageBoxService();
        var doc  = new MaxCharsDoc(fake);
        using var form = new Form();
        form.Controls.Add(new TextBox { Name = "tb1", Text = "toolong" }); // 7 > 5
        doc.AttachView(form);

        var result = doc.UpdateData(true);

        Assert.False(result);
        Assert.Single(fake.Calls);
        Assert.Equal(MessageBoxIcon.Warning, fake.Calls[0].Icon);
    }

    // ── Calls before AttachView ────────────────────────────────────────────

    [StaFact]
    public void UpdateData_BeforeAttachView_ReturnsFalse()
    {
        var doc = new TestDoc();

        Assert.False(doc.UpdateData(false));
        Assert.False(doc.UpdateData(true));
    }

    // ── GetDdxMappings ───────────────────────────────────────────────────────

    [StaFact]
    public void GetDdxMappings_BeforeAttachView_ReturnsEmpty()
    {
        var doc = new TestDoc();

        Assert.Empty(doc.GetDdxMappings());
    }

    [StaFact]
    public void GetDdxMappings_AfterAttachView_ReturnsAllBoundMembers()
    {
        var doc = new TestDoc();
        using var form = CreateForm();
        doc.AttachView(form);

        var mappings = doc.GetDdxMappings();

        Assert.Equal(3, mappings.Count);
        Assert.Contains(mappings, m => m.ControlName == "tb1"  && m.MemberName == nameof(TestDoc.StrField));
        Assert.Contains(mappings, m => m.ControlName == "cb1"  && m.MemberName == nameof(TestDoc.BoolField));
        Assert.Contains(mappings, m => m.ControlName == "nud1" && m.MemberName == nameof(TestDoc.IntField));
    }

    // ── ResourceId control operations ──────────────────────────────────────

    [StaFact]
    public void AttachResourceId_GetControl_ReturnsMatchedControl()
    {
        var doc = new ResourceDoc();
        using var form = new Form();
        form.Controls.Add(new TextBox { Name = "tb1" });
        doc.AttachView(form);
        doc.AttachIds();

        Assert.NotNull(doc.GetTb1());
    }

    [StaFact]
    public void SetEnabled_DisablesControl()
    {
        var doc = new ResourceDoc();
        using var form = new Form();
        var tb = new TextBox { Name = "tb1", Enabled = true };
        form.Controls.Add(tb);
        doc.AttachView(form);
        doc.AttachIds();

        doc.DisableTb1();

        Assert.False(tb.Enabled);
    }

    [StaFact]
    public void SetVisible_HidesControl()
    {
        var doc = new ResourceDoc();
        using var form = new Form();
        var tb = new TextBox { Name = "tb1", Visible = true };
        form.Controls.Add(tb);
        doc.AttachView(form);
        doc.AttachIds();

        doc.HideTb1();

        Assert.False(tb.Visible);
    }

    [StaFact]
    public void SetVisible_ShowsHiddenControl()
    {
        var doc = new ResourceDoc();
        using var form = new Form();
        var tb = new TextBox { Name = "tb1" };
        form.Controls.Add(tb);
        doc.AttachView(form);
        doc.AttachIds();

        doc.HideTb1();
        doc.ShowTb1(); // restore ctrl.Visible = true

        // When the form is hidden, Visible follows the parent chain; remove from parent
        // to assert the control's own internal visible state
        form.Controls.Remove(tb);
        Assert.True(tb.Visible);
    }

    [StaFact]
    public void GetControl_BeforeAttachResourceId_ReturnsNull()
    {
        var doc = new ResourceDoc();
        using var form = new Form();
        form.Controls.Add(new TextBox { Name = "tb1" });
        doc.AttachView(form);
        // Do not call AttachIds()

        Assert.Null(doc.GetTb1());
    }

    // ── GetDdxState ──────────────────────────────────────────────────────────

    [StaFact]
    public void GetDdxState_BeforeAttachView_ReturnsEmpty()
    {
        var doc = new TestDoc();

        Assert.Empty(doc.GetDdxState());
    }

    [StaFact]
    public void GetDdxState_AfterUpdateDataFalse_AllInSync()
    {
        var doc = new TestDoc { StrField = "hello", BoolField = true, IntField = 42 };
        using var form = CreateForm();
        doc.AttachView(form);
        doc.UpdateData(false); // Doc → UI

        var state = doc.GetDdxState();

        Assert.Equal(3, state.Count);
        Assert.All(state, s => Assert.True(s.InSync));
    }

    [StaFact]
    public void GetDdxState_WhenDocDiffersFromUI_ShowsOutOfSync()
    {
        // Form has tb1.Text = "fromUI" but doc.StrField stays "docValue"
        var doc = new TestDoc { StrField = "docValue" };
        using var form = CreateForm(); // tb1.Text = "fromUI"
        doc.AttachView(form);
        // Skip UpdateData(false) → UI and document differ

        var state = doc.GetDdxState();

        var tb1 = state.First(s => s.ControlName == "tb1");
        Assert.False(tb1.InSync);
        Assert.Equal("fromUI",  tb1.ControlValue);
        Assert.Equal("docValue", tb1.DocumentValue);
    }

    [StaFact]
    public void GetDdxState_MissingControl_ReportsControlValueAsDash()
    {
        var doc = new TestDoc();
        using var form = new Form(); // no controls added
        doc.AttachView(form);

        var state = doc.GetDdxState();

        // Missing control → ControlValue is em dash
        Assert.All(state, s => Assert.Equal("—", s.ControlValue));
    }

    // ── Helper document for DDV failure tests ───────────────────────────────

    private class MaxCharsDoc : MfcDocument
    {
        public MaxCharsDoc(IMessageBoxService? svc = null) : base(svc ?? new FakeMessageBoxService()) { }

        [DDX("tb1")]
        [DDVMaxChars(5)]
        public string ShortField = "";
    }

    // ── Document for property DDX tests ────────────────────────────────────

    private class PropertyDoc : MfcDocument
    {
        public PropertyDoc() : base(new FakeMessageBoxService()) { }

        [DDX("tb1")]
        public string NameProp { get; set; } = "initial";
    }

    // ── Property DDX ───────────────────────────────────────────────────────

    [StaFact]
    public void UpdateData_False_Property_TransfersDocumentToUI()
    {
        var doc = new PropertyDoc { NameProp = "docValue" };
        using var form = new Form();
        form.Controls.Add(new TextBox { Name = "tb1" });
        doc.AttachView(form);

        doc.UpdateData(false);

        Assert.Equal("docValue", ((TextBox)form.Controls["tb1"]!).Text);
    }

    [StaFact]
    public void UpdateData_True_Property_TransfersUIToDocument()
    {
        var doc = new PropertyDoc();
        using var form = new Form();
        form.Controls.Add(new TextBox { Name = "tb1", Text = "newValue" });
        doc.AttachView(form);

        doc.UpdateData(true);

        Assert.Equal("newValue", doc.NameProp);
    }

    [StaFact]
    public void GetDdxMappings_Property_IsIncluded()
    {
        var doc = new PropertyDoc();
        using var form = new Form();
        form.Controls.Add(new TextBox { Name = "tb1" });
        doc.AttachView(form);

        var mappings = doc.GetDdxMappings();

        Assert.Single(mappings);
        Assert.Equal("tb1", mappings[0].ControlName);
        Assert.Equal(nameof(PropertyDoc.NameProp), mappings[0].MemberName);
    }
}
