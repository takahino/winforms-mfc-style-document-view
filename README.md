# Windows Forms Document–View

A small framework that brings the **MFC Document–View** style to **.NET WinForms**. It replaces `DoDataExchange` / `DDX_*` / `DDV_*` with **C# attributes** on document fields, so you can keep business logic in the document class and lower the cost of porting MFC dialogs.

**Languages:** [English](README.md) · [日本語](README.ja.md)

## Features

- **`MfcDocument`** — `AttachView`, `UpdateData(true|false)` mirroring MFC's save/validate and load-to-UI behavior
- **Declarative binding** — `[DDX(...)]` and `[DDV*]` attributes instead of handwritten exchange code
- **Roslyn Source Generator** — `partial` document classes get a compile-time `BuildEntries()` override; direct field access replaces reflection in the hot path (reflection fallback for non-`partial` classes)
- **`MfcWinApp`** — application entry similar to `CWinApp` / `InitInstance` + message loop via `Run()`
- **`IMessageBoxService`** — abstracted `MessageBox` for tests and mocks
- **Optional `ResourceId` + `[AutoId]`** — numbered control IDs aligned with `resource.h` style when you need them

## Requirements

- **Windows** (WinForms)
- **.NET 10** (`net10.0-windows`) with Windows Forms enabled
- **Visual Studio 2022** (or another IDE) with the .NET 10 SDK workload

## Repository layout

| Path | Role |
|------|------|
| `DocumentView.Framework/` | Reusable framework (`MfcWinApp`, `MfcDocument`, DDX/DDV attributes, converters) |
| `DocumentView.Framework.Generator/` | Roslyn Incremental Source Generator — generates delegate-based `BuildEntries()` for `partial` document classes |
| `DocumentView.Sample/` | Sample 1 — Employee information app (single Document) |
| `DocumentView.Sample2/` | Sample 2 — Purchase order management app (3 Documents attached to one Form) |
| `DocumentView.Framework.Tests/` | Framework unit tests |
| `DocumentView.Sample.Tests/` | Sample 1 unit tests |
| `DocumentView.Sample2.Tests/` | Sample 2 unit tests |
| `docs/architecture.md` | Architecture overview (English) |
| `docs/architecture.ja.md` | Architecture overview (Japanese) |
| `MFC/` | **Reference only** — VC++ 6.0–style MFC sketch mapped to the C# sample (not a guaranteed build) |

## Build and run

```bash
dotnet build WindowsFormsDocumentView.slnx

# Sample 1 — Employee information
dotnet run --project DocumentView.Sample/DocumentView.Sample.csproj

# Sample 2 — Purchase order management
dotnet run --project DocumentView.Sample2/DocumentView.Sample2.csproj
```

Each sample registers services with `Microsoft.Extensions.DependencyInjection`, resolves `MfcWinApp`, and calls `Run()`.

## Screenshots

**Sample 1 — Employee information**

The window includes a debug panel that shows DDX binding state (control ↔ document fields) and an operation log for `UpdateData` / validation, similar to tracing MFC-style dialog data exchange.

![Employee information sample with DDX debug panel](docs/employee-information-sample.png)

**Sample 2 — Purchase order management**

Demonstrates porting an MFC SDI app with **three separate dialogs** (`IDD_SUPPLIER_INFO`, `IDD_ORDER_HEADER`, `IDD_ORDER_DETAIL`) into a single WinForms window. Three independent `MfcDocument` subclasses are all attached to the same `Form`. The debug panel aggregates DDX state from all three documents.

## Quick example (concept)

`MfcDocument` requires an `IMessageBoxService` (constructor injection; the sample wires it via `Microsoft.Extensions.DependencyInjection`). Document fields are annotated; the framework moves values between controls and fields when you call `UpdateData`.

Add `partial` to opt in to zero-reflection code generation by the Source Generator:

```csharp
public partial class SampleDocument : MfcDocument   // partial → generator produces BuildEntries()
{
    public SampleDocument(IMessageBoxService messageBoxService) : base(messageBoxService) { }

    [DDX(SampleView.Ctrl.IDC_EDIT_NAME)]
    [DDVMaxChars(30)]
    public string m_strName = string.Empty;

    [DDX(SampleView.Ctrl.IDC_EDIT_AGE)]
    [DDVMinMax(0, 150)]
    public int m_nAge = 0;
}
```

Omitting `partial` is safe — the reflection-based fallback in `MfcDocument.BuildEntries()` continues to work unchanged.

See `DocumentView.Sample/SampleDocument.cs`, `SampleView.cs`, and `Program.cs` for the full pattern (DI, grid, and buttons).

For the **multiple-Document pattern** (Sample 2), see [docs/architecture.md](docs/architecture.md#multiple-documents-on-one-form).

## Documentation

- [docs/architecture.md](docs/architecture.md) — project structure, MFC mapping table, responsibilities (English)
- [docs/architecture.ja.md](docs/architecture.ja.md) — same content in Japanese
- [MFC/README.md](MFC/README.md) — how the optional MFC folder relates to the C# sample

## License

This project is licensed under the [BSD 3-Clause License](LICENSE).
