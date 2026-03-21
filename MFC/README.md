# VC++ 6.0 MFC implementation aligned with `DocumentView.Sample`

This folder shows how the C# WinForms sample in this repository (`DocumentView.Sample`) can be structured with **Visual C++ 6.0 + MFC (SDI + `CFormView`)**. It contains source and resources only; no prebuilt binaries.

**The contents are a sample to illustrate design ideas—Document / View, DDX-like wiring, resource ID alignment with the C# side, and so on. The repository does not include VC++ 6.0 project files, and nothing here is guaranteed to build and run as-is on your machine. Do not treat this as an executable, production-ready program.**

## Conceptual mapping

| C# (WinForms) | This MFC sample |
|---------------|-----------------|
| `SampleWinApp` / `MfcWinApp` | `CEmployeeSampleApp` (`CSingleDocTemplate` in `CWinApp::InitInstance`) |
| `SampleView : Form` | `CSampleView : CFormView` (`IDD_SAMPLE_FORM`) |
| `SampleDocument : MfcDocument` | `CSampleDoc : CDocument` |
| `[DDX]` / `[DDV*]` + `UpdateData` | `GetDlgItem` / `SetDlgItemText` and hand-written validation inside `CSampleDoc::UpdateData` |
| `ResourceId` (`GetControl` / `SetEnabled`) | `IDC_*` in `resource.h` and `GetControl` / `SetEnabled` |
| `DataGridView` (project list) | `SysListView32` (report mode). Synced with the document on save/load (live cell edits into the document are not implemented) |
| Right pane (DDX state / operation log) | **Omitted** (this sample focuses on the business flow) |
| `employees\*.json` (UTF-8) | Same path. Writes UTF-8 with BOM; reads UTF-8 and converts to the current ANSI code page |

## Reference: integrating into your own project (VC++ 6.0)

These steps are not a verified build recipe; they are a rough guide when porting this source into your own MFC project.

1. Create a new project with **MFC AppWizard (exe)**: SDI, **CFormView**-based (name is up to you, e.g. `EmployeeSample`).
2. Among the `.cpp` / `.h` / `.rc` files the wizard generates, **replace or supersede** the ones that correspond to the logic in this folder with the files provided here (add these files and remove wizard output as needed).
3. **Project settings**: “Use of MFC” should be **Use MFC in a Shared DLL** (or static); character set **Multi-Byte Character Set (MBCS)** is recommended (this sample’s `ReadUtf8File` / `WriteUtf8File` include `_UNICODE` branches).
4. Include these files in the project:
   - `StdAfx.cpp` / `StdAfx.h`
   - `EmployeeSample.cpp` / `EmployeeSample.h`
   - `MainFrm.cpp` / `MainFrm.h`
   - `SampleDoc.cpp` / `SampleDoc.h`
   - `SampleView.cpp` / `SampleView.h`
   - `Sample.rc` / `resource.h`
5. **Precompiled headers**: only `StdAfx.cpp` should **create** (`/Yc`); all others should **use** (`/Yu`).

## Resource IDs (alignment with C# `ResourceId`)

The numbering for `IDC_EDIT_NAME` through `IDC_EDIT_EMPNO` in `resource.h` matches the C# side’s `AutoId` sequence (1001, …).

## Notes

- JSON read/write is a **minimal implementation without external libraries**. It is more limited than C#’s `System.Text.Json`, but is intended to be compatible with the object shape the sample produces.
- Even if you edit values in the list view, they are mainly pulled into the document when you **OK (save)** or at **`UpdateData(TRUE)`-like** moments.
