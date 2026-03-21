# VC++ 6.0 MFC 版 `DocumentView.Sample` 対応実装例

このフォルダは、リポジトリ内の C# WinForms サンプル（`DocumentView.Sample`）を **Visual C++ 6.0 + MFC（SDI + `CFormView`）** で組んだ場合の構成例です。ビルド済みバイナリは含めず、ソースとリソースのみです。

**本フォルダの内容は、C# 側の Document / View・DDX 相当・リソース ID の対応など、設計イメージを伝えるためのサンプルです。リポジトリには VC++ 6.0 のプロジェクトファイルを含めておらず、手元の環境でそのままビルドして動作を保証するものではありません。実際に実行可能なプログラムとして扱わないでください。**

## 対応関係（概念）

| C#（WinForms） | 本 MFC 例 |
|----------------|-----------|
| `SampleWinApp` / `MfcWinApp` | `CEmployeeSampleApp`（`CWinApp::InitInstance` で `CSingleDocTemplate`） |
| `SampleView : Form` | `CSampleView : CFormView`（`IDD_SAMPLE_FORM`） |
| `SampleDocument : MfcDocument` | `CSampleDoc : CDocument` |
| `[DDX]` / `[DDV*]` + `UpdateData` | `CSampleDoc::UpdateData` 内の `GetDlgItem` / `SetDlgItemText` と手書き検証 |
| `ResourceId`（`GetControl` / `SetEnabled`） | `resource.h` の `IDC_*` と `GetControl` / `SetEnabled` |
| `DataGridView`（プロジェクト一覧） | `SysListView32`（レポートモード）。保存・読込時にドキュメントと同期（セル編集のライブ反映は未実装） |
| 右ペイン（DDX 状態・操作ログ） | **省略**（本例は業務フロー中心） |
| `employees\*.json`（UTF-8） | 同パス。BOM 付き UTF-8 で書き出し、読み込みは UTF-8 → 現在の ANSI コードページに変換 |

## 参考：プロジェクトに取り込む場合の手順（VC++ 6.0）

動作確認済みの手順ではなく、あくまでソースを自分の MFC プロジェクトへ移植するときの目安です。

1. **MFC AppWizard (exe)** で SDI、**CFormView** ベースのプロジェクトを新規作成する（名前は任意。例: `EmployeeSample`）。
2. ウィザードが生成した `.cpp` / `.h` / `.rc` のうち、**本フォルダの同名ロジックに相当するファイル**を、ここに置いた内容で置き換えるか、ファイルをプロジェクトに追加してウィザード生成を削除する。
3. **プロジェクト設定**: 「Microsoft Foundation Classes の使用」が **MFC を共有 DLL で使用**（または静的）になっていること、文字セットは **マルチバイト文字セット (MBCS)** を推奨（本サンプルの `ReadUtf8File` / `WriteUtf8File` は `_UNICODE` 分岐あり）。
4. 次のファイルをプロジェクトに含める:
   - `StdAfx.cpp` / `StdAfx.h`
   - `EmployeeSample.cpp` / `EmployeeSample.h`
   - `MainFrm.cpp` / `MainFrm.h`
   - `SampleDoc.cpp` / `SampleDoc.h`
   - `SampleView.cpp` / `SampleView.h`
   - `Sample.rc` / `resource.h`
5. **プリコンパイル済みヘッダ**: `StdAfx.cpp` のみ「作成 (`/Yc`）」、他は「使用 (`/Yu`）」。

## Resource ID（C# `ResourceId` との対応）

`resource.h` の `IDC_EDIT_NAME` ～ `IDC_EDIT_EMPNO` の番号付けは、C# 側の `AutoId` 連番（1001 ～）に合わせています。

## 注意

- JSON の読み書きは **外部ライブラリなし**の簡易実装です。C# の `System.Text.Json` より制限がありますが、サンプルが出力するオブジェクト形とは互換を意図しています。
- リストビュー上の値を編集した場合でも、ドキュメントへ取り込むのは主に **OK（保存）** や **`UpdateData(TRUE)` 相当**を呼ぶタイミングです。
