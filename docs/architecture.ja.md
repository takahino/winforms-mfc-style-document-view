# アーキテクチャ概要

**言語:** [English](architecture.md) · [日本語](architecture.ja.md)

MFC の Document-View パターンを WinForms に移植するためのフレームワーク。
MFC の `DoDataExchange` / `DDX_*` / `DDV_*` を C# のアトリビュートで宣言的に置き換え、
ロジックを Document に集約することで MFC コードの移植コストを最小化する。

---

## プロジェクト構成

```
WindowsFormsDocumentView/
├── DocumentView.Framework/      ← 移植フレームワーク本体（再利用可能）
│   ├── MfcWinApp.cs             ← CWinApp 相当（メインフォーム起動）
│   ├── MfcDocument.cs           ← CDocument 相当の基底クラス
│   ├── DDXAttribute.cs          ← [DDX] アトリビュート
│   ├── DDVAttribute.cs          ← [DDV*] アトリビュート群
│   ├── AutoIdAttribute.cs       ← [AutoId] アトリビュート（ResourceId 自動採番用）
│   ├── AutoIdAssigner.cs        ← [AutoId] フィールドへの連番付与ヘルパー
│   ├── ResourceIdResolver.cs    ← resource.h 定数クラスのリフレクション解析
│   ├── ControlValueConverter.cs ← コントロール ↔ CLR 型の値変換
│   └── IMessageBoxService.cs    ← MessageBox の抽象化（テスト用モック注入可）
├── DocumentView.Framework.Generator/  ← Roslyn Incremental Source Generator
│   ├── DdxSourceGenerator.cs    ← IIncrementalGenerator エントリポイント
│   ├── DdxSymbolModel.cs        ← Roslyn シンボルから収集したデータモデル
│   └── DdxEntryEmitter.cs       ← ソースコード生成ロジック
├── DocumentView.Sample/         ← サンプル 1：従業員情報（Document 1 つ）
│   ├── ResourceId.cs            ← resource.h 相当（[AutoId] で自動採番）
│   ├── SampleWinApp.cs          ← MfcWinApp 実装（メインは SampleView）
│   ├── SampleDocument.cs        ← CDocument 派生クラスの移植例（partial）
│   ├── SampleView.cs            ← Form（イベントを Document に委譲）
│   └── SampleView.Designer.cs
├── DocumentView.Sample2/        ← サンプル 2：発注管理（Document 3 つ）
│   ├── ResourceId.cs            ← resource.h 相当（IDC_* 定数 13 個）
│   ├── PurchaseOrderWinApp.cs   ← MfcWinApp 実装（メインは OrderView）
│   ├── OrderLine.cs             ← 発注明細行（INotifyPropertyChanged）
│   ├── SupplierDocument.cs      ← IDD_SUPPLIER_INFO 相当（partial）
│   ├── OrderHeaderDocument.cs   ← IDD_ORDER_HEADER 相当（partial）
│   ├── OrderDetailDocument.cs   ← IDD_ORDER_DETAIL 相当（partial）
│   ├── OrderView.cs             ← Form（3 つの Document をアタッチ）
│   └── OrderView.Designer.cs
├── DocumentView.Framework.Tests/ ← Framework 単体テスト
├── DocumentView.Sample.Tests/    ← サンプル 1 の単体テスト
└── DocumentView.Sample2.Tests/   ← サンプル 2 の単体テスト
```

---

## クラス図

### サンプル 1 — 従業員情報

```
DocumentView.Framework
┌─────────────────────────────────┐
│ MfcWinApp (abstract)            │
│  + Run()                         │
│  # CreateMainForm() : Form       │
└─────────────────────────────────┘
         ▲
         │ 派生（アプリごと）
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
│  # BuildEntries() : List<…>     │──→ virtual; Generator が override を生成
│  ─ _view : Control?             │
│  ─ _ddxEntries                  │──→ List<DdxEntry>（デリゲートベース）
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
                                   [コンパイル時に自動生成]
                                   SampleDocument.DdxGenerated.g.cs
                                   override BuildEntries() { return [ … ]; }
```

### サンプル 2 — 発注管理（1 つの Form に 3 Document をアタッチ）

```
DocumentView.Sample2
┌──────────────────────────────────────────────────────────────┐
│  OrderView : Form                                            │
│                                                              │
│  SupplierDoc ───────────────────→ SupplierDocument          │
│  HeaderDoc   ───────────────────→ OrderHeaderDocument        │
│  DetailDoc   ───────────────────→ OrderDetailDocument        │
│                                                              │
│  コンストラクタ:                                              │
│    SupplierDoc.AttachView(this)  ← 3 つが同一 Form を共有   │
│    HeaderDoc.AttachView(this)                                │
│    DetailDoc.AttachView(this)                                │
│    [3×] AttachResourceId(typeof(ResourceId))                 │
│                                                              │
│  OnNew() ──→ SupplierDoc.Reset()                            │
│              HeaderDoc.Reset(orderNo)                        │
│              DetailDoc.Reset() + SubscribeGridLines()        │
│                                                              │
│  OnBtnSave() ──→ [3×] UpdateData(true) + ValidateBusiness   │
│  btnCancel_Click ──→ [3×] UpdateData(false)                 │
└──────────────────────────────────────────────────────────────┘

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

## MFC との対応表

| MFC | このフレームワーク |
|-----|-----------------|
| `resource.h` の `#define IDC_*` | `ResourceId` に `[AutoId(XxxView.Ctrl.IDC_*)]` 付き `static int` フィールドを並べ、宣言順に連番 ID を付与する（IDC 値が必要なビジネスロジック向け）。従来の `const int` も利用可。 |
| `DoDataExchange` の `DDX_Text(pDX, IDC_X, m_x)` | `[DDX(XxxView.Ctrl.IDC_X)]` アトリビュート（View の `nameof` アクセッサ経由で指定） |
| `DoDataExchange` の `DDV_MaxChars(pDX, m_x, 30)` | `[DDVMaxChars(30)]` アトリビュート |
| `UpdateData(TRUE)` / `UpdateData(FALSE)` | `Document.UpdateData(true)` / `Document.UpdateData(false)` |
| `GetDocument()` | `Document` プロパティ（View が所有） |
| `OnBnClickedOk → GetDocument()->OnBtnOk()` | `btnOk_Click` → `Document.OnBtnOk()` |
| `CWinApp::InitInstance`（メインウィンドウ表示とメッセージループ） | `MfcWinApp.Run()`（`CreateMainForm()` で解決した `Form` に対する `Application.Run`） |

---

## ファイル別の責務

### `MfcWinApp.cs`

MFC の `CWinApp` に相当するアプリケーションオブジェクトの基底クラス。

```
protected MfcWinApp(IServiceProvider serviceProvider)
  └─ CreateMainForm() 用に ServiceProvider を保持

Run()
  └─ Application.Run(CreateMainForm())

CreateMainForm()   ← 派生クラスで実装（通常は GetRequiredService<メインForm>()）
```

派生クラスは DI で構築した `IServiceProvider` を基底コンストラクタに渡す（`SampleWinApp` 参照）。`AddSingleton<MfcWinApp, XxxWinApp>()` のようにシングルトン登録し、`Program.Main` から `GetRequiredService<MfcWinApp>().Run()` する。

### `MfcDocument.cs`

フレームワークのコア。DDX/DDV の実行エンジン。`[DDX]` / `[DDV*]` は **インスタンスのフィールドまたはプロパティ** に付与できる。

```
AttachView(Form view)
  └─ _view と _ddxEntries を BuildEntries() 経由で初期化

BuildEntries() : List<DdxEntry>   ← protected virtual
  · デフォルト（リフレクション fallback）: [DDX]/[DDV*] 付きフィールド/プロパティを走査し、
    GetValue/SetValue をラムダでラップして DdxEntry を構築
  · override（Generator 生成）: 直接フィールドアクセス — () => this.m_strName など

UpdateData(saveAndValidate: true)   ← UI → Document + DDV バリデーション
  1. DDX フェーズ: entry.Setter(newVal) で各フィールドに書き込む
  2. DDV フェーズ: entry.Validators foreach → 失敗時: フォーカス + MessageBox + return false

UpdateData(saveAndValidate: false)  ← Document → UI
  DDX フェーズのみ: ControlValueConverter.SetValue(ctrl, entry.Getter(), …)
```

`DdxEntry` は `protected record` で、メンバー名・型・コントロール名・`Getter`・`Setter`・事前計算済み `Validators` リストを保持する。ホットパスに `MemberInfo` 参照は存在しない。

任意の診断用 API: イベント `DebugLog` / `DataUpdated`、`GetDdxState()`（バインドごとに UI 値とドキュメント値を比較。サンプルのデバッググリッドで使用）。

`MfcDocument` にパラメータなしコンストラクタはない。DI またはテストから `IMessageBoxService` を渡す（`protected MfcDocument(IMessageBoxService messageBoxService)`）。

### `DocumentView.Framework.Generator/`

Roslyn Incremental Source Generator（`netstandard2.0`）。消費プロジェクトから Analyzer として参照する。

**対象条件:** `MfcDocument` を（直接または間接に）継承する `partial` クラス。

**生成物:** `{クラス名}.DdxGenerated.g.cs` — `BuildEntries()` を override する `partial class` ファイル。直接フィールドアクセスのラムダと再構築した `DDV*` アトリビュートインスタンスをコレクション式で返す。

```csharp
// 生成コード例
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

`partial` を付けないクラスはリフレクションベースの `BuildEntries()` fallback がそのまま動作するため、破壊的変更はない。

Generator をプロジェクトから参照するには:
```xml
<ProjectReference Include="..\DocumentView.Framework.Generator\DocumentView.Framework.Generator.csproj"
                  OutputItemType="Analyzer"
                  ReferenceOutputAssembly="false" />
```

### `DDXAttribute.cs`

`[DDX(ViewClass.Ctrl.IDC_*)]` で Document のフィールドまたはプロパティとコントロール名（`Control.Name`）を紐づける。
`ControlProperty` で対象プロパティを明示指定可（省略時はメンバ型から自動判定）。

### `DDVAttribute.cs`

バリデーションルールを定義するアトリビュート基底クラス。実装クラス：

| アトリビュート | MFC 相当 | 説明 |
|--------------|---------|------|
| `[DDVMinMax(min, max)]` | `DDV_MinMaxInt` / `DDV_MinMaxDouble` | 数値範囲チェック |
| `[DDVMaxChars(n)]` | `DDV_MaxChars` | 文字列最大長チェック |

具体的な DDV アトリビュートは、省略可能な `Message` で既定のエラーメッセージを上書きできる。

### `AutoIdAttribute.cs` / `AutoIdAssigner.cs`

`[AutoId(controlName)]` を付けた `static int` フィールドに宣言順の連番 ID を自動付与する仕組み。
`controlName` に `SampleView.Ctrl.IDC_*` 定数を渡すことで、ResourceId フィールドとコントロールの対応をアノテーションで明示する。
`ResourceId` の static コンストラクタから `AutoIdAssigner.Assign(typeof(ResourceId))` を呼ぶことで初期化される。

### `ResourceIdResolver.cs`

`ResourceId` クラスのリフレクション解析。`int (IDC 値) ↔ string (コントロール名)` の双方向変換。
`[AutoId(controlName)]` のコントロール名を優先し、省略時はフィールド名をフォールバックとして使用する。
`const int`（旧来形式）と `[AutoId] static int`（自動採番形式）の両方に対応。
DDX バインディングには使用しない。Document のビジネスロジックが IDC 値を必要とする場合（MFC 互換処理など）に利用するユーティリティ。

### `IMessageBoxService.cs`

`MessageBox.Show` を抽象化したインターフェース。`MfcDocument` にコンストラクタ注入する。

```csharp
public interface IMessageBoxService
{
    DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon);
    // 省略形（デフォルト実装）
    DialogResult Show(string text) => ...;
    DialogResult Show(string text, string caption) => ...;
}
```

- **本番**: コンポジションルートで `AddSingleton<IMessageBoxService, MessageBoxService>()` 等として登録し、`SampleDocument` を `AddTransient` で解決するとコンストラクタ注入される（`DocumentView.Sample/Program.cs` 参照）
- **テスト**: `FakeMessageBoxService` を渡して `new SampleDocument(fakeService)` のように生成する

`MfcDocument` の派生クラスでは `MessageBoxService.Show(...)` を呼ぶ。

### `ControlValueConverter.cs`

コントロール型に応じた値の読み書きを担う静的クラス。
`UpdateData` から呼ばれる。拡張する場合は `switch` に `case` を追加する。

| コントロール | 読み取り値 | 書き込み対象 |
|-------------|----------|------------|
| `TextBox` | `Text` | `Text` |
| `CheckBox` / `RadioButton` | `Checked` | `Checked` |
| `NumericUpDown` | `Value` | `Value` (Clamp 適用) |
| `ComboBox` (int フィールド) | `SelectedIndex` | `SelectedIndex` |
| `ComboBox` (string フィールド) | `Text` | `Text` |
| `ComboBox`（`ControlProperty = "SelectedItem"`） | `SelectedItem` | `SelectedItem` |
| `ListBox` (int フィールド) | `SelectedIndex` | `SelectedIndex` |
| `DataGridView` | `DataSource` | `DataSource` (同一参照なら再バインドしない) |
| `Label` | `Text` | `Text` |

---

## View と Document の責務分担

```
SampleView (Form)                          SampleDocument (MfcDocument)
─────────────────────────────────────────  ──────────────────────────────────────
・InitializeComponent()                    ・m_* フィールド + [DDX][DDV] アトリビュート
・コンボ/リストの Items 初期設定            ・UpdateData(true/false) ← MfcDocument から継承
・Document.AttachView(this)               ・OnBtnOk / OnBtnCancel / OnBtnDebug / OnBtnShowGrid / OnNew / OnMenuLoad / OnCheckActiveChanged
・OnLoad → Document.UpdateData(false)      ・ValidateBusinessRule
・イベントハンドラ → Document への 1 行委譲  ・ビジネスロジック全般
・（サンプル）DebugLog / DataUpdated → デバッグ UI
```

View は UI の初期化と委譲のみ。ビジネスロジックは一切持たない。

---

## 1 つの Form に複数の Document をアタッチする

MFC アプリでは複数のダイアログを 1 つのウィンドウに組み合わせることがある。サンプル 2 はこのパターンの移植例で、かつてのダイアログごとに `MfcDocument` サブクラスを作り、3 つすべてを同一の `Form` にアタッチする。

```csharp
// OrderView コンストラクタ
SupplierDoc.AttachView(this);   // 3 つが同一 Form を共有
HeaderDoc  .AttachView(this);
DetailDoc  .AttachView(this);

// 各 Document が同じ ResourceId を参照（FindControl は子孫全体を検索）
SupplierDoc.AttachResourceId(typeof(ResourceId));
HeaderDoc  .AttachResourceId(typeof(ResourceId));
DetailDoc  .AttachResourceId(typeof(ResourceId));
```

**主なルール:**

- `FindControl` は内部で `searchAllChildren: true` を使用するため、`SplitContainer` や `GroupBox` の中にネストされたコントロールも正しく発見される。
- 各 Document の `[DDX]` フィールドは、共有 `ResourceId` 内で一意である必要がある。3 つの Document はすべて同一の `ResourceId` 型を参照する。
- コーディネーター（View）は保存時に各 Document の `UpdateData(true)` を順番に呼び出す。初期化・復元時の `UpdateData(false)` は各 Document が `Reset()` / `RestoreFrom()` の中で自己責任で呼び出す。

### Reset / RestoreFrom パターン

フィールドの初期化と UI への反映は Document 自身の責任とする：

```csharp
// SupplierDocument
public void Reset()
{
    m_strSupplierCode = string.Empty;
    // ... 他のフィールド ...
    UpdateData(false);   // Document が自分で UI に反映
}

public void RestoreFrom(PurchaseOrderData d)
{
    m_strSupplierCode = d.SupplierCode;
    // ...
    UpdateData(false);   // Document が自分で UI に反映
}
```

View 側の `OnNew()` と `FromData()` は、各 Document の `Reset()` / `RestoreFrom()` を呼ぶだけで、初期化目的の `UpdateData` は直接呼ばない。

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

`UpdateData(true)` は保存時のみ View が呼び出す。複数 Document にまたがる検証順序の調整は View の責務である。

---

## 新しい画面を追加する手順

### 1. ResourceId を定義する（IDC 値が必要な場合）

先に手順 3 の `MyView.Ctrl` が必要。各フィールドに `[AutoId(コントロール名定数)]` を付け、static コンストラクタで `AutoIdAssigner.Assign` を呼ぶと宣言順に連番が入る。

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

### 2. Document を作成する

`partial` を付けるとコンパイル時コード生成が有効になる（推奨）。`partial` なしでも動作し、その場合はリフレクション fallback が適用される。

```csharp
public partial class MyDocument : MfcDocument   // partial → Generator がリフレクションを排除
{
    public MyDocument(IMessageBoxService messageBoxService) : base(messageBoxService) { }

    [DDX(MyView.Ctrl.IDC_EDIT_NAME)]
    [DDVMaxChars(30)]
    public string m_strName = string.Empty;

    public void OnBtnOk()
    {
        if (!UpdateData(true)) return;
        // ビジネスロジック
        UpdateData(false);
    }
}
```

### 3. View を作成する（Form を直接継承）

サンプルと同様、Document は `new` せず DI から注入する（基底クラスが `IMessageBoxService` を要求するため）。

```csharp
public partial class MyView : Form
{
    public MyDocument Document { get; }

    /// <summary>WinForms デザイナ用。</summary>
    public MyView() : this(null!) { }

    public MyView(MyDocument document)
    {
        Document = document;
        InitializeComponent();
        if (document is null) return;

        Document.AttachView(this);
        Document.AttachResourceId(typeof(ResourceId)); // GetControl / SetEnabled 等で使う場合
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        Document.UpdateData(false);
    }

    private void btnOk_Click(object sender, EventArgs e) => Document.OnBtnOk();
}
```

### 4. サービス登録（コンポジションルート）

```csharp
services.AddSingleton<IMessageBoxService, MessageBoxService>();
services.AddTransient<MyDocument>();
services.AddTransient<MyView>();
services.AddSingleton<MfcWinApp, MyWinApp>(); // CreateMainForm → GetRequiredService<MyView>()
```

`DocumentView.Sample/Program.cs` および `SampleWinApp.cs` を参照。

### 5. プロジェクトに Generator 参照を追加する

```xml
<!-- MyApp.csproj -->
<ItemGroup>
  <ProjectReference Include="..\DocumentView.Framework.Generator\DocumentView.Framework.Generator.csproj"
                    OutputItemType="Analyzer"
                    ReferenceOutputAssembly="false" />
</ItemGroup>
```

**ルール:**
- View の `SampleView.cs` に `static class Ctrl` を定義し、Designer の private フィールドを `nameof` で公開する
- `GetControl` / `SetEnabled` 等で数値 ID を使う場合は、`AttachView` のあと `AttachResourceId(typeof(ResourceId))` を呼ぶ
- `[DDX(XxxView.Ctrl.IDC_EDIT_NAME)]` でバインド（タイポはコンパイルエラーになる）
- `[DDX]` と `[DDV*]` は同じフィールドに複数付与可
- `DataGridView` は `BindingList<T>` を `[DDX]` でマップするとライブバインディングになる

---

## DDX 型マッピングの自動判定ルール

`ControlProperty` を省略した場合の自動判定：

| フィールド型 | コントロール | 対象プロパティ |
|------------|------------|-------------|
| `int` | `ComboBox` / `ListBox` | `SelectedIndex` |
| `string` | `ComboBox` / `ListBox` | `Text` |
| `bool` | `CheckBox` / `RadioButton` | `Checked` |
| 任意 | `DataGridView` | `DataSource` |

明示指定例: `[DDX(SampleView.Ctrl.IDC_COMBO_PREF, ControlProperty = "SelectedItem")]`

---

## DataGridView のライブバインディング

`BindingList<T>` を DDX マップした場合の動作：

```
UpdateData(false) → dgv.DataSource = m_gridItems   ← 初回バインド
                                                      以降はグリッド編集が即時 m_gridItems に反映
UpdateData(true)  → no-op（DataSource は既に同一参照）
```

グリッド操作は `UpdateData(true)` を呼ばずとも Document に即時反映される。
`OnBtnShowGrid` でその動作を確認できる。
