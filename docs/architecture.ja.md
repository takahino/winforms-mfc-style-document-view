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
├── DocumentView.Sample/         ← 使用例
│   ├── ResourceId.cs            ← resource.h 相当（[AutoId] で自動採番）
│   ├── SampleWinApp.cs          ← MfcWinApp 実装（メインは SampleView）
│   ├── SampleDocument.cs        ← CDocument 派生クラスの移植例
│   ├── SampleView.cs            ← Form（イベントを Document に委譲）
│   └── SampleView.Designer.cs
├── DocumentView.Framework.Tests/ ← Framework 単体テスト
└── DocumentView.Sample.Tests/    ← Sample 単体テスト
```

---

## クラス図

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
│  ─ _view : Control?             │
│  ─ _ddxEntries                  │──→ DDXAttribute / DDVAttribute
└─────────────────────────────────┘

DocumentView.Sample
┌──────────────────┐     has-a     ┌────────────────────────┐
│  SampleView      │──────────────→│  SampleDocument        │
│  : Form          │               │  : MfcDocument         │
│                  │               │                        │
│  OnLoad          │               │  m_strName [DDX][DDV]  │
│  btnOk_Click ────┼─ delegates ──→│  OnBtnOk()             │
│  btnCancel_Click ┼─ delegates ──→│  OnBtnCancel()         │
│  btnDebug_Click ─┼─ delegates ──→│  OnBtnDebug()          │
│  btnShowGrid_Click┼─ delegates ──→│  OnBtnShowGrid()      │
└──────────────────┘               └────────────────────────┘
```

---

## MFC との対応表

| MFC | このフレームワーク |
|-----|-----------------|
| `resource.h` の `#define IDC_*` | `ResourceId` クラスの `const int` フィールド（Document のビジネスロジックが IDC 値を必要とする場合のみ使用） |
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
Run()
  └─ Application.Run(CreateMainForm())

CreateMainForm()   ← 派生クラスで実装（通常は GetRequiredService<メインForm>()）
```

DI では `AddSingleton<MfcWinApp, XxxWinApp>()` のようにシングルトン登録し、`Program.Main` から `GetRequiredService<MfcWinApp>().Run()` する。

### `MfcDocument.cs`

フレームワークのコア。DDX/DDV の実行エンジン。

```
AttachView(Form view)
  └─ _view と _ddxEntries を初期化（[DDX("IDC_*")] のコントロール名を直接使用）

UpdateData(saveAndValidate: true)   ← UI → Document + DDV バリデーション
  1. DDX フェーズ: 各コントロールの値を対応 m_* フィールドに書き込む
  2. DDV フェーズ: [DDV*] アトリビュートで値を検証
               → 失敗時: コントロールにフォーカス移動 + MessageBox + return false

UpdateData(saveAndValidate: false)  ← Document → UI
  DDX フェーズのみ: 各 m_* フィールドの値を対応コントロールに書き込む
```

### `DDXAttribute.cs`

`[DDX(ViewClass.Ctrl.IDC_*)]` で Document フィールドとコントロール名（`Control.Name` プロパティ）を紐づける。
`ControlProperty` で対象プロパティを明示指定可（省略時は型から自動判定）。

### `DDVAttribute.cs`

バリデーションルールを定義するアトリビュート基底クラス。実装クラス：

| アトリビュート | MFC 相当 | 説明 |
|--------------|---------|------|
| `[DDVMinMax(min, max)]` | `DDV_MinMaxInt` / `DDV_MinMaxDouble` | 数値範囲チェック |
| `[DDVMaxChars(n)]` | `DDV_MaxChars` | 文字列最大長チェック |

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

- **本番**: `new SampleDocument()` → デフォルトで `MessageBoxService`（実際の MessageBox）を使用
- **テスト**: `new SampleDocument(fakeService)` → `FakeMessageBoxService` を注入して MessageBox を抑制

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
・Document.AttachView(this)               ・OnBtnOk / OnBtnCancel / OnBtnDebug / OnBtnShowGrid
・OnLoad → Document.UpdateData(false)      ・ValidateBusinessRule
・イベントハンドラ → Document への 1 行委譲  ・ビジネスロジック全般
```

View は UI の初期化と委譲のみ。ビジネスロジックは一切持たない。

---

## 新しい画面を追加する手順

### 1. ResourceId を定義する

```csharp
public static class ResourceId
{
    public const int IDC_EDIT_NAME = 1001;
    // ...
}
```

### 2. Document を作成する

```csharp
public class MyDocument : MfcDocument
{
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

```csharp
public partial class MyView : Form
{
    public MyDocument Document { get; } = new();

    public MyView()
    {
        InitializeComponent();
        Document.AttachView(this);
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        Document.UpdateData(false);
    }

    private void btnOk_Click(object sender, EventArgs e) => Document.OnBtnOk();
}
```

**ルール:**
- View の `SampleView.cs` に `static class Ctrl` を定義し、Designer の private フィールドを `nameof` で公開する
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
