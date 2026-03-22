# Architecture overview

**Languages:** [English](architecture.md) · [日本語](architecture.ja.md)

A framework for porting MFC's Document–View pattern to WinForms.
It replaces MFC `DoDataExchange` / `DDX_*` / `DDV_*` with declarative C# attributes on document fields and keeps logic in the document, minimizing the cost of porting MFC code.

---

## Project layout

```
WindowsFormsDocumentView/
├── DocumentView.Framework/      ← Portable framework core
│   ├── MfcWinApp.cs             ← CWinApp equivalent (main form + startup)
│   ├── MfcDocument.cs           ← CDocument-style base class
│   ├── DDXAttribute.cs          ← [DDX] attribute
│   ├── DDVAttribute.cs          ← [DDV*] attributes
│   ├── AutoIdAttribute.cs       ← [AutoId] (ResourceId auto-numbering)
│   ├── AutoIdAssigner.cs        ← Helper that assigns sequential IDs
│   ├── ResourceIdResolver.cs    ← Reflection over resource.h-style constants
│   ├── ControlValueConverter.cs ← Control ↔ CLR value conversion
│   └── IMessageBoxService.cs    ← MessageBox abstraction (mockable in tests)
├── DocumentView.Framework.Generator/  ← Roslyn Incremental Source Generator
│   ├── DdxSourceGenerator.cs    ← IIncrementalGenerator entry point
│   ├── DdxSymbolModel.cs        ← Data models collected from Roslyn symbols
│   └── DdxEntryEmitter.cs       ← Source code generation logic
├── DocumentView.Sample/         ← Sample 1: Employee information (single Document)
│   ├── ResourceId.cs            ← resource.h equivalent ([AutoId] numbering)
│   ├── SampleWinApp.cs          ← MfcWinApp impl (main = SampleView)
│   ├── SampleDocument.cs        ← Example CDocument-style class (partial)
│   ├── SampleView.cs            ← Form (delegates events to document)
│   └── SampleView.Designer.cs
├── DocumentView.Sample2/        ← Sample 2: Purchase order management (3 Documents)
│   ├── ResourceId.cs            ← resource.h equivalent (13 IDC_* constants)
│   ├── PurchaseOrderWinApp.cs   ← MfcWinApp impl (main = OrderView)
│   ├── OrderLine.cs             ← Order line row (INotifyPropertyChanged)
│   ├── SupplierDocument.cs      ← IDD_SUPPLIER_INFO equivalent (partial)
│   ├── OrderHeaderDocument.cs   ← IDD_ORDER_HEADER equivalent (partial)
│   ├── OrderDetailDocument.cs   ← IDD_ORDER_DETAIL equivalent (partial)
│   ├── OrderView.cs             ← Form (attaches all 3 documents)
│   └── OrderView.Designer.cs
├── DocumentView.Framework.Tests/ ← Framework unit tests
├── DocumentView.Sample.Tests/    ← Sample 1 unit tests
└── DocumentView.Sample2.Tests/   ← Sample 2 unit tests
```

---

## Class diagram

### Sample 1 — Employee information

```
DocumentView.Framework
┌─────────────────────────────────┐
│ MfcWinApp (abstract)            │
│  + Run()                         │
│  # CreateMainForm() : Form       │
└─────────────────────────────────┘
         ▲
         │ derived (per app)
DocumentView.Sample
┌─────────────────────────────────┐
│ SampleWinApp : MfcWinApp         │
└─────────────────────────────────┘

DocumentView.Framework
┌─────────────────────────────────┐
│ MfcDocument (abstract)          │
│  + AttachView(Form)             │
│  + UpdateData(bool) : bool      │
│  + GetDdxMappings()             │
│  + GetDdxState()                │
│  ◇ DebugLog / DataUpdated       │
│  # BuildEntries() : List<…>     │──→ virtual; overridden by generator
│  ─ _view : Control?             │
│  ─ _ddxEntries                  │──→ List<DdxEntry> (delegate-based)
└─────────────────────────────────┘

DocumentView.Sample
┌──────────────────┐     has-a     ┌────────────────────────┐
│  SampleView      │──────────────→│  SampleDocument        │
│  : Form          │               │  : MfcDocument         │
│                  │               │  (partial)             │
│  OnLoad          │               │  m_strName [DDX][DDV]  │
│  btnOk_Click ────┼─ delegates ──→│  OnBtnOk()             │
│  btnCancel_Click ┼─ delegates ──→│  OnBtnCancel()         │
│  btnDebug_Click ─┼─ delegates ──→│  OnBtnDebug()          │
│  btnShowGrid_Click┼─ delegates ──→│  OnBtnShowGrid()      │
└──────────────────┘               └────────────────────────┘
                                             ▲
                                   [generated at compile time]
                                   SampleDocument.DdxGenerated.g.cs
                                   override BuildEntries() { return [ … ]; }
```

### Sample 2 — Purchase order management (3 Documents on one Form)

```
DocumentView.Sample2
┌─────────────────────────────────────────────────────────────┐
│  OrderView : Form                                           │
│                                                             │
│  SupplierDoc ──────────────────→ SupplierDocument          │
│  HeaderDoc   ──────────────────→ OrderHeaderDocument        │
│  DetailDoc   ──────────────────→ OrderDetailDocument        │
│                                                             │
│  constructor:                                               │
│    SupplierDoc.AttachView(this)  ← all 3 share same Form   │
│    HeaderDoc.AttachView(this)                               │
│    DetailDoc.AttachView(this)                               │
│    [3×] AttachResourceId(typeof(ResourceId))                │
│                                                             │
│  OnNew() ──→ SupplierDoc.Reset()                           │
│              HeaderDoc.Reset(orderNo)                       │
│              DetailDoc.Reset() + SubscribeGridLines()       │
│                                                             │
│  OnBtnSave() ──→ [3×] UpdateData(true) + ValidateBusiness  │
│  btnCancel_Click ──→ [3×] UpdateData(false)                │
└─────────────────────────────────────────────────────────────┘

SupplierDocument     OrderHeaderDocument    OrderDetailDocument
: MfcDocument        : MfcDocument          : MfcDocument
(partial)            (partial)              (partial)
─────────────────    ──────────────────     ─────────────────────
m_strSupplierCode    m_strOrderNo           m_gridLines [DDX]
m_strSupplierName    m_strOrderDate         m_strTotal  [DDX]
m_strAddress         m_strDeliveryDate
m_strTel             m_nStatus [DDX]        RecalculateTotal()
m_strFax             m_bUrgent  [DDX]       SubscribeGridLines()
                     m_strMemo  [DDX]
Reset()              Reset(orderNo)         Reset()
RestoreFrom(d)       RestoreFrom(d)         RestoreFrom(d)
ValidateBusiness()   OnStatusChanged()
```

---

## MFC mapping

| MFC | This framework |
|-----|----------------|
| `#define IDC_*` in `resource.h` | `ResourceId` with `[AutoId(XxxView.Ctrl.IDC_*)] static int` fields (sequential IDs in declaration order; only when business logic needs numeric IDs). Classic `const int` is still supported. |
| `DDX_Text(pDX, IDC_X, m_x)` in `DoDataExchange` | `[DDX(XxxView.Ctrl.IDC_X)]` (via View `nameof` accessors) |
| `DDV_MaxChars(pDX, m_x, 30)` | `[DDVMaxChars(30)]` |
| `UpdateData(TRUE)` / `UpdateData(FALSE)` | `Document.UpdateData(true)` / `Document.UpdateData(false)` |
| `GetDocument()` | `Document` property (owned by the view) |
| `OnBnClickedOk → GetDocument()->OnBtnOk()` | `btnOk_Click` → `Document.OnBtnOk()` |
| `CWinApp::InitInstance` (main window + message loop) | `MfcWinApp.Run()` → `Application.Run` on the form from `CreateMainForm()` |

---

## Responsibilities by file

### `MfcWinApp.cs`

Base application object equivalent to MFC `CWinApp`.

```
protected MfcWinApp(IServiceProvider serviceProvider)
  └─ stores ServiceProvider for CreateMainForm()

Run()
  └─ Application.Run(CreateMainForm())

CreateMainForm()   ← implement in derived type (often GetRequiredService<MainForm>())
```

Derived types pass the DI-built `IServiceProvider` to the base constructor (see `SampleWinApp`). Register as a singleton, e.g. `AddSingleton<MfcWinApp, XxxWinApp>()`, and call `GetRequiredService<MfcWinApp>().Run()` from `Program.Main`.

### `MfcDocument.cs`

Core DDX/DDV engine. `[DDX]` / `[DDV*]` may be applied to **instance fields or properties** (reflection scans both).

```
AttachView(Form view)
  └─ Initialize _view and _ddxEntries via BuildEntries()

BuildEntries() : List<DdxEntry>   ← protected virtual
  · Default (reflection fallback): scans fields/properties with [DDX] / [DDV*]
    and wraps GetValue/SetValue in Getter/Setter delegates
  · Override (generated): direct field access — () => this.m_strName, etc.

UpdateData(saveAndValidate: true)   ← UI → Document + DDV
  1. DDX: entry.Setter(newVal) for each bound field
  2. DDV: entry.Validators foreach → on failure: focus + MessageBox + return false

UpdateData(saveAndValidate: false)  ← Document → UI
  DDX only: ControlValueConverter.SetValue(ctrl, entry.Getter(), …)
```

`DdxEntry` is a `protected record` holding the member name/type, control name, `Getter`, `Setter`, and pre-computed `Validators` list. No `MemberInfo` references remain in the hot path.

Optional diagnostics: `DebugLog` and `DataUpdated` events; `GetDdxState()` compares each bound control with its document member (used by the sample's debug grid).

`MfcDocument` has no parameterless constructor—pass `IMessageBoxService` from DI or tests (`protected MfcDocument(IMessageBoxService messageBoxService)`).

### `DocumentView.Framework.Generator/`

Roslyn Incremental Source Generator (`netstandard2.0`). Referenced as an Analyzer from consumer projects.

**Trigger:** any `partial` class that (directly or indirectly) inherits `MfcDocument`.

**Output:** `{ClassName}.DdxGenerated.g.cs` — a `partial class` that overrides `BuildEntries()` with a collection expression of `DdxEntry` objects using direct field/property access lambdas and reconstructed `DDV*` attribute instances.

```csharp
// generated example
partial class SampleDocument
{
    protected override List<MfcDocument.DdxEntry> BuildEntries() =>
    [
        new MfcDocument.DdxEntry(
            MemberName: "m_strName", MemberType: typeof(string),
            ControlName: "IDC_EDIT_NAME", ControlProperty: null,
            Getter: () => this.m_strName,
            Setter: v  => this.m_strName = (string)v,
            Validators: [ new DDVMaxCharsAttribute(30) ]),
        // …
    ];
}
```

Non-`partial` classes continue to use the reflection-based `BuildEntries()` fallback — no breaking change.

To reference the generator from a project:
```xml
<ProjectReference Include="..\DocumentView.Framework.Generator\DocumentView.Framework.Generator.csproj"
                  OutputItemType="Analyzer"
                  ReferenceOutputAssembly="false" />
```

### `DDXAttribute.cs`

`[DDX(ViewClass.Ctrl.IDC_*)]` binds a document field or property to `Control.Name`.
Optional `ControlProperty` overrides the target property; otherwise inferred from member type.

### `DDVAttribute.cs`

Base for validation attributes. Concrete types:

| Attribute | MFC equivalent | Purpose |
|-----------|----------------|---------|
| `[DDVMinMax(min, max)]` | `DDV_MinMaxInt` / `DDV_MinMaxDouble` | Numeric range |
| `[DDVMaxChars(n)]` | `DDV_MaxChars` | Max string length |

Concrete DDV attributes accept an optional `Message` property to override the default error text.

### `AutoIdAttribute.cs` / `AutoIdAssigner.cs`

`[AutoId(controlName)]` on `static int` fields gets sequential IDs in declaration order.
Pass `SampleView.Ctrl.IDC_*` constants to document which control each ID maps to.
Call `AutoIdAssigner.Assign(typeof(ResourceId))` from the `ResourceId` static constructor.

### `ResourceIdResolver.cs`

Reflects a `ResourceId` type for bidirectional `int (IDC value) ↔ string (control name)`.
Prefers `[AutoId(controlName)]`; falls back to the field name if omitted.
Supports both `const int` (classic) and `[AutoId] static int` (auto-numbered).
Not used for DDX binding—only for document code that needs MFC-style numeric IDs (`GetControl` / `SetEnabled` / etc.).

### `IMessageBoxService.cs`

Abstraction over `MessageBox.Show`, constructor-injected into `MfcDocument`.

```csharp
public interface IMessageBoxService
{
    DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon);
    // Short overloads (default interface methods)
    DialogResult Show(string text) => ...;
    DialogResult Show(string text, string caption) => ...;
}
```

- **Production**: DI resolves `MessageBoxService` (real dialogs).
- **Tests**: pass `FakeMessageBoxService` to suppress UI.

Derived documents should call `MessageBoxService.Show(...)`, not `MessageBox.Show` directly.

### `ControlValueConverter.cs`

Static helper for reading/writing control values; called from `UpdateData`.
Add `switch` cases to support more control types.

| Control | Read | Write |
|---------|------|-------|
| `TextBox` | `Text` | `Text` |
| `CheckBox` / `RadioButton` | `Checked` | `Checked` |
| `NumericUpDown` | `Value` | `Value` (clamped) |
| `ComboBox` (int field) | `SelectedIndex` | `SelectedIndex` |
| `ComboBox` (string field) | `Text` | `Text` |
| `ComboBox` (`ControlProperty = "SelectedItem"`) | `SelectedItem` | `SelectedItem` |
| `ListBox` (int field) | `SelectedIndex` | `SelectedIndex` |
| `DataGridView` | `DataSource` | `DataSource` (skip rebind if same reference) |
| `Label` | `Text` | `Text` |

---

## View vs document responsibilities

```
SampleView (Form)                          SampleDocument (MfcDocument)
─────────────────────────────────────────  ──────────────────────────────────────
· InitializeComponent()                    · m_* fields + [DDX][DDV]
· Combo/list Items setup                   · UpdateData(true/false) from MfcDocument
· Document.AttachView(this)                · OnBtnOk / OnBtnCancel / OnBtnDebug / OnBtnShowGrid / OnNew / OnMenuLoad / OnCheckActiveChanged
· OnLoad → Document.UpdateData(false)      · ValidateBusinessRule
· One-line handlers → document methods       · All business logic
· Optional: DebugLog / DataUpdated → debug UI (sample)
```

The view only initializes UI and delegates. It holds no business logic.

---

## Multiple Documents on one Form

MFC applications sometimes combine several dialogs in a single window. Sample 2 shows how to port this pattern: each former dialog becomes a separate `MfcDocument` subclass, and all three are attached to the same `Form`.

```csharp
// OrderView constructor
SupplierDoc.AttachView(this);   // all three share the same Form
HeaderDoc  .AttachView(this);
DetailDoc  .AttachView(this);

// Each document maps its own fields; FindControl searches all children
SupplierDoc.AttachResourceId(typeof(ResourceId));
HeaderDoc  .AttachResourceId(typeof(ResourceId));
DetailDoc  .AttachResourceId(typeof(ResourceId));
```

**Key rules:**

- `FindControl` uses `searchAllChildren: true` internally, so controls nested inside `SplitContainer` panels or `GroupBox` are found correctly.
- Each document's `[DDX]` field names must be unique within the shared `ResourceId`; all three documents reference the same `ResourceId` type.
- The coordinator (view) calls `UpdateData(true)` on each document in sequence for save; each document calls `UpdateData(false)` on itself inside `Reset()` and `RestoreFrom()`.

### Reset / RestoreFrom pattern

Document methods own the responsibility for initialising their fields **and** pushing them to the UI:

```csharp
// SupplierDocument
public void Reset()
{
    m_strSupplierCode = string.Empty;
    // ... other fields ...
    UpdateData(false);   // Document pushes to UI itself
}

public void RestoreFrom(PurchaseOrderData d)
{
    m_strSupplierCode = d.SupplierCode;
    // ...
    UpdateData(false);   // Document pushes to UI itself
}
```

The view's `OnNew()` and `FromData()` simply call `Reset()` / `RestoreFrom()` on each document — they do not call `UpdateData` directly for initialization.

```csharp
// OrderView
public void OnNew()
{
    SupplierDoc.Reset();
    HeaderDoc  .Reset(GenerateOrderNo());
    DetailDoc  .Reset();
    DetailDoc  .SubscribeGridLines();
}
```

`UpdateData(true)` remains the view's responsibility (called during save), because only the view coordinates cross-document validation order.

---

## Adding a new screen

### 1. Define `ResourceId` (if you need numeric IDs)

`MyView.Ctrl` must exist first (step 3). Map each ID to a control name via `[AutoId(...)]`; IDs are assigned in field declaration order when the static constructor runs.

```csharp
using DocumentView.Framework;

namespace MyApp;

public static class ResourceId
{
    [AutoId(MyView.Ctrl.IDC_EDIT_NAME)] public static int IDC_EDIT_NAME;
    // ...

    static ResourceId() => AutoIdAssigner.Assign(typeof(ResourceId));
}
```

### 2. Create the document

Add `partial` to enable compile-time code generation (recommended). The class works without `partial` too — reflection fallback applies.

```csharp
public partial class MyDocument : MfcDocument   // partial → generator eliminates reflection
{
    public MyDocument(IMessageBoxService messageBoxService) : base(messageBoxService) { }

    [DDX(MyView.Ctrl.IDC_EDIT_NAME)]
    [DDVMaxChars(30)]
    public string m_strName = string.Empty;

    public void OnBtnOk()
    {
        if (!UpdateData(true)) return;
        // business logic
        UpdateData(false);
    }
}
```

### 3. Create the view (`Form` subclass)

Match the sample: inject the document from DI instead of `new MyDocument()` (the base class requires `IMessageBoxService`).

```csharp
public partial class MyView : Form
{
    public MyDocument Document { get; }

    /// <summary>For the WinForms designer.</summary>
    public MyView() : this(null!) { }

    public MyView(MyDocument document)
    {
        Document = document;
        InitializeComponent();
        if (document is null) return;

        Document.AttachView(this);
        Document.AttachResourceId(typeof(ResourceId)); // if using GetControl / SetEnabled / …
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        Document.UpdateData(false);
    }

    private void btnOk_Click(object sender, EventArgs e) => Document.OnBtnOk();
}
```

### 4. Register services (composition root)

```csharp
services.AddSingleton<IMessageBoxService, MessageBoxService>();
services.AddTransient<MyDocument>();
services.AddTransient<MyView>();
services.AddSingleton<MfcWinApp, MyWinApp>(); // CreateMainForm → GetRequiredService<MyView>()
```

See `DocumentView.Sample/Program.cs` and `SampleWinApp.cs`.

### 5. Add the Generator reference to the project

```xml
<!-- MyApp.csproj -->
<ItemGroup>
  <ProjectReference Include="..\DocumentView.Framework.Generator\DocumentView.Framework.Generator.csproj"
                    OutputItemType="Analyzer"
                    ReferenceOutputAssembly="false" />
</ItemGroup>
```

**Rules:**
- Add `static class Ctrl` on the view with `nameof` for each designer control used in DDX.
- After `AttachView`, call `AttachResourceId(typeof(ResourceId))` when the document uses numeric IDs (`GetControl`, `SetEnabled`, etc.).
- Use `[DDX(XxxView.Ctrl.IDC_EDIT_NAME)]` so typos fail at compile time.
- Multiple `[DDX]` / `[DDV*]` on the same field are allowed.
- `DataGridView` + `BindingList<T>` + `[DDX]` gives live binding.

---

## DDX type inference (when `ControlProperty` is omitted)

| Field type | Control | Property |
|------------|---------|----------|
| `int` | `ComboBox` / `ListBox` | `SelectedIndex` |
| `string` | `ComboBox` / `ListBox` | `Text` |
| `bool` | `CheckBox` / `RadioButton` | `Checked` |
| any | `DataGridView` | `DataSource` |

Explicit example: `[DDX(SampleView.Ctrl.IDC_COMBO_PREF, ControlProperty = "SelectedItem")]`

---

## DataGridView live binding

With `BindingList<T>` mapped through DDX:

```
UpdateData(false) → dgv.DataSource = m_gridItems   ← first bind
                                                      later edits go straight to m_gridItems
UpdateData(true)  → no-op (same DataSource reference)
```

Grid edits reach the document without calling `UpdateData(true)`.
Try `OnBtnShowGrid` in the sample to see it.
